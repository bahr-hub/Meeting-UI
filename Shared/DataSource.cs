using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Shared
{
    public class Filter
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Operator { get; set; }
    }
    public class DataSource
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = int.MaxValue;
        public List<Filter> Filter { get; set; }

        public DataSourceResult ToResult<TType>(IQueryable<TType> Data)
        {
            if (this.Page == 0)
                return new DataSourceResult
                {
                    Data = new List<TType>(),
                    Count = 0
                };
            if (this.Filter != null && this.Filter.Count > 0)
            {

                Expression FinalExpression = null;
                ParameterExpression parameter = Expression.Parameter(typeof(TType));
                // we will loop through the filters
                foreach (var filter in Filter)
                {
                    // find the property of a given name
                    var property = typeof(TType).GetProperty(filter.Key, BindingFlags.Instance | BindingFlags.Public);
                    if (property == null) continue;

                    // create the ParameterExpression
                    //parameter = Expression.Parameter(typeof(TType));
                    // and use that expression to get the expression of a property
                    // like: x.SampleProperty1
                    var memberExpression = Expression.Property(parameter, property);

                    // Convert object type to the actual type of the property
                    var value = Convert.ChangeType(filter.Value, property.PropertyType, CultureInfo.InvariantCulture);

                    if (value != null)
                    {
                        // Construct equal expression that compares MemberExpression for the property with converted value
                        var eq = Expression.Call(memberExpression, typeof(string).GetMethod("Contains", new[] { typeof(string) }), Expression.Constant(value));

                        // Build lambda expresssion (x => x.SampleProperty == some-value)
                        // var lambdaExpression = Expression.Lambda<Func<TType, bool>>(eq, parameter);
                        if (FinalExpression == null)
                            FinalExpression = eq;
                        else
                            FinalExpression = Expression.And(FinalExpression, eq);
                        // And finally use the expression to filter the collection

                        var lambdaExpression = Expression.Lambda<Func<TType, bool>>(FinalExpression, parameter);
                        Data = Data.Where(lambdaExpression);
                    }

                }
            }
            try
            {
                return new DataSourceResult
                {

                    Data = Data.Skip((this.Page - 1) * this.PageSize).Take(PageSize).AsQueryable(),
                    Count = Data.Count()
                };
            }
            catch (Exception ex)
            {
                return null;
            }

        }
    }

    public class DataSourceResult
    {
        public dynamic Data { get; set; }
        public long Count { get; set; }
    }

}

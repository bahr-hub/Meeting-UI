using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Model.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Shared;
using System;

namespace Services.Services
{
    public class TaskyAuth
    {
        public string email { get; set; }
        public string password { get; set; }
    }
    public class TaskyTask
    {

        public taskyresponsible responsibleID { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public int sourceId { get; set; }
        public int priorityId { get; set; }
        public Boolean? reminderEnabled { get; set; }
        public string reminderDate { get; set; }
        public string assignorUserGroupId { get; set; }
        public string assignorEntityUserId { get; set; }

    }

    public class taskyresponsible
    {
        public string id { get; set; }
        public string roleID { get; set; }
        public int type { get; set; }
    }
  public   class TaskyIntegrationService 
    {
        public User CurrentUser { get; set; }
        public string TaskyHost { get; set; }
        public static string Token { get; set; }
        public TaskyIntegrationService (User _user , string Host)
        {
            CurrentUser = _user;
            TaskyHost = Host;
        }

        public string TaskyAuthenticate()
        {
            /************************add task to tasky***************************
                 * ***********************this code for demo , must refactor ***************
                 * *****************************************************************/
            // Get Token of Authenticate user
            TaskyAuth _AuthObject = new TaskyAuth
            {
                email = CurrentUser.Name,
                password = Shared.Security.TripleDES.Decrypt(CurrentUser.Password, true)
            };

            var client = new RestClient(TaskyHost + "/Account/Authenticate");
            client.Timeout = -1;
            var requestAuth = new RestRequest(Method.POST);
            requestAuth.AddHeader("Content-Type", "application/json-patch+json");
            requestAuth.AddParameter("application/json-patch+json", JsonConvert.SerializeObject(_AuthObject), ParameterType.RequestBody);

            IRestResponse response = client.Execute(requestAuth);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                JObject joResponse = JObject.Parse(response.Content);

                var token = joResponse["token"].ToString();
                return token;
            }
            else return "";
        }

        public string CreateTask(MeetingTask Task)
        {
            if (Token == null || Token =="")
            {
                Token = TaskyAuthenticate();
            }
            // Create  task in tasky system
            if (Token != null)
            {
                var client = new RestClient(TaskyHost + "/Task/Create");
                TaskyTask __task = new TaskyTask
                {
                    responsibleID = new taskyresponsible
                    {
                        id = Task.AssigneeId.ToString(),
                        roleID = "1027",
                        type = 0
                    },
                    title = Task.Name,
                    description = null,
                    startDate = DateTime.Now,
                    endDate = Task.DueDate,
                    sourceId = 2013,
                    priorityId = 3,
                    reminderEnabled = null,
                    reminderDate = "1970-01-01T02:00:00.000Z",
                    assignorUserGroupId = "aa9f0864-f8be-4593-81a7-4c9f00f81cb2",
                    assignorEntityUserId = null
                };

                var request = new RestRequest(Method.POST);
                request.AddHeader("accept", "text/plain");
                request.AddHeader("Authorization", " Bearer " + Token);
                request.AddHeader("Content-Type", "application/json-patch+json");
                request.AddParameter("application/json-patch+json", JsonConvert.SerializeObject(__task), ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var TaskID = (JObject.Parse(response.Content)["data"] != null) ? JObject.Parse(response.Content)["data"]["id"].ToString() : null;
                    /****************************/
                    return TaskID;
                }
            }
            return null;
        }

        public string GetTaskStatus(string TaskId)
        {
            if (TaskId != null && TaskId != "")
            {


                if (Token == null || Token == "")
                {
                    Token = TaskyAuthenticate();
                }
                if (Token != null)
                {
                    var client = new RestClient(TaskyHost + "/Task/GetTasksStatus");
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("accept", "text/plain");
                    request.AddHeader("Authorization", " Bearer " + Token);
                    request.AddHeader("Content-Type", "application/json-patch+json");
                    request.AddParameter("application/json-patch+json", "[\r\n  \"" + TaskId + "\"\r\n]", ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        if (((Newtonsoft.Json.Linq.JContainer)JObject.Parse(response.Content)["data"]).Count > 0)
                        {
                            var Status = JObject.Parse(response.Content)["data"][0]["customStatusID"].ToString();

                            return Status;
                        }
                    }
                }
            }
             return null;
        }

        public string MarkTaskAsCompleted(string TaskId)
        {
            if (TaskId != null && TaskId != "")
            {
                if (Token == null || Token == "")
                {
                    Token = TaskyAuthenticate();
                }
                if (Token != null)
                {
                    var client = new RestClient(TaskyHost + "/Task/MarkTaskAsCompleted?Taskid=" + TaskId);
                    client.Timeout = -1;
                    var request = new RestRequest(Method.GET);
                    request.AddHeader("accept", "text/plain");
                    request.AddHeader("Authorization", " Bearer " + Token);
                    IRestResponse response = client.Execute(request);
                    return "";
                }
            }
            return null;
        }
    }
}

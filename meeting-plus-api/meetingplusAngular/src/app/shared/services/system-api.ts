export interface IResult {
    success: boolean;
    data?: any | undefined;
    message?: string | undefined;
    count: number;
}


export class Result implements IResult {
    success!: boolean;
    data?: any | undefined;
    message?: string | undefined;
    count!: number;

    constructor(data?: IResult) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(data?: any) {
        if (data) {
            this.success = data["success"];
            this.data = data["data"];
            this.message = data["message"];
            this.count = data["count"];
        }
    }

    static fromJS(data: any): Result {
        data = typeof data === 'object' ? data : {};
        let result = new Result();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["success"] = this.success;
        data["data"] = this.data;
        data["message"] = this.message;
        data["count"] = this.count;
        return data; 
    }
}
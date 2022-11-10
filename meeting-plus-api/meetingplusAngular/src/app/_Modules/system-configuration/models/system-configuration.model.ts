export class SystemConfigurationModel {
    public id: number;
    public authenticationMode: number;
    public integrationWithGoogleCalendar: boolean;
    public maxMeetingTime: number;
    public startOfWorkDays: number;
    public endOfWorkDays: number;
    public timeZone: string;
}
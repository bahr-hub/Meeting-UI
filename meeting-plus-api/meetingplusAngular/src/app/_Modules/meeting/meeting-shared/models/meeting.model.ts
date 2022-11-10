export class MeetingModel {
    id: any;
    name: string;
    locationId: number;
    projectId: number;
    status: number;
    from: Date;
    to: Date;
    startedAt: Date;
    endedAt: Date;
    postponedTo: Date;
    notes: string;
    isDeleted: boolean;
    createdBy: number;
    createdAt: Date;
    updatedBy: any;
    updatedAt: Date;
    externalToken: string;
    previousMeetingID: any;
    meetingTopic: any = [];
    meetingTask: any = [];
    meetingTag: any = [];
    meetingParticipant: any = [];
    createdByNavigation: any;
    location: any;
    project: any; 
    updatedByNavigation: any;
}

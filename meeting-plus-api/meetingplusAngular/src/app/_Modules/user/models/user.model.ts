import { UserProfile } from './user-profile.model';
import { UserConfiguration } from './user-configuration.model';

export class UserModel {
    id: string;
    name: string;
    email: string;
    password?: string;
    confirmPassword?: string;
    mobile: string;
    fkUserProfileId?: string;
    isSuperAdmin: boolean;
    isAdmin: boolean;
    isActive: boolean;
    token?: string;
    photo?: string;
    vacationFkUser?: any;
    userRole?: any;
    fkUserProfile: UserProfile = new UserProfile;
    fkUserConfiguration?: UserConfiguration = new UserConfiguration;
    ability?: any;
    imageBase64: string;
    imageUrl: string;
    locationID:number;
}

import { UserDataModel } from './user-data.model';

export class AccessDataModel {
    accessToken: string;
    expiresInSeconds: number;
    refreshToken: string;
    userData: UserDataModel;
}

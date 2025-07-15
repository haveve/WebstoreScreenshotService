export enum ScreenshotType {
    Png = 'Png',
    Jpeg = 'Jpeg'
}

export enum SubscriptionType {
    Regular = 1,
}

export type SubscriptionPlan = {
    type: SubscriptionType;
    screenshotLeft: number;
}

export type UserModel = {
    name: string;
    surname: string;
    email: string;
    subscriptionPlan: SubscriptionPlan;
}

export type LoginModel = {
    email: string;
    password: string;
}

export type RegisterModel = {
    email: string;
    password: string;
    name: string;
    surname: string;
}

export interface Clip {
    width: number;
    height: number | null;
}

export interface ScreenshotOptionsModel {
    url: string;
    screenshotType: ScreenshotType;
    clip: Clip;
}
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

// Enums
export enum ScreenshotType {
    Png = 'Png',
    Jpeg = 'Jpeg'
}

export enum ScreenshotQualityMode {
    High = 'High',
    Medium = 'Medium',
    Low = 'Low'
}

export enum ColorSchemeOption {
    Light = 'Light',
    Dark = 'Dark',
    NoPreference = 'NoPreference'
}

// Flags enum (bitmask)
export enum ResourceBlockOptions {
    None = 0,
    Images = 1 << 0,
    Fonts = 1 << 1,
    Media = 1 << 2,
    Scripts = 1 << 3,
    Stylesheets = 1 << 4,
    All = Images | Fonts | Media | Scripts | Stylesheets
}

// Models
export interface ClipModel {
    width: number;
    height?: number | null;
}

export interface ElementModel {
    selector: string;
}

export interface ModalModel {
    dismissDialogs: boolean;
    hidePopups: boolean;
    hideSelectors: string[];
}

export type HighlightWordModel = {
    word: string;
    color: string;
}

export type HeaderModel = {
    name: string;
    value: string;
}

export type CookieModel = {
    name: string;
    value: string;
    domain: string;
    path: string;
    expires?: string;
    secure: boolean;
    httpOnly: boolean;
    sameSite?: 'Strict' | 'Lax' | 'None';
}

export type AdvancedConfigurationModel = {
    locale: string;
    timezoneId: string;
    colorScheme: ColorSchemeOption;
    waitForSelector?: string | null;
    blockResources: ResourceBlockOptions;
    headers: HeaderModel[];
    cookies: CookieModel[];
}

export type ScreenshotOptionsModel = {
    url: string;
    screenshotType: ScreenshotType;
    mode: ScreenshotQualityMode;
    clip?: ClipModel;
    element?: ElementModel;
    modalModel?: ModalModel;
    highlightWord?: HighlightWordModel;
    advancedConfiguration?: AdvancedConfigurationModel;
}

export type Screenshot = {
    id: string;
    url: string;
    websiteUrl: string,
    userId: string,
    createdAt: string,
    state: ScreenshotState,
    type: ScreenshotType,
    title: string | null,
    description: string | null,
}

export enum ScreenshotState
{
    New = 'New',
    Successful = 'Successful',
    Failed = 'Failed',
}
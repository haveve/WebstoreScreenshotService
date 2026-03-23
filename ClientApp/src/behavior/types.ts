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

export interface HighlightWordModel {
    word: string;
    color: string;
}

export interface HeaderModel {
    name: string;
    value: string;
}

export interface CookieModel {
    name: string;
    value: string;
    domain: string;
    path: string;
    expires?: string;
    secure: boolean;
    httpOnly: boolean;
    sameSite?: 'Strict' | 'Lax' | 'None';
}

export interface AdvancedConfigurationModel {
    locale: string;
    timezoneId: string;
    colorScheme: ColorSchemeOption;
    waitForSelector?: string | null;
    blockResources: ResourceBlockOptions;
    headers: HeaderModel[];
    cookies: CookieModel[];
}

export interface ScreenshotOptionsModel {
    url: string;
    screenshotType: ScreenshotType;
    mode: ScreenshotQualityMode;

    clip?: ClipModel;
    element?: ElementModel;
    modalModel?: ModalModel;
    highlightWord?: HighlightWordModel;
    advancedConfiguration?: AdvancedConfigurationModel;
}
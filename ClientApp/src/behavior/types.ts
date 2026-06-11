export enum SubscriptionType {
    Regular = 1,
    Pro = 2,
    Advanced = 3,
}

export enum Duration {
  Monthly = 0,
  Yearly = 1
}

export type SubscriptionPlan = {
    type: SubscriptionType;
    duration: Duration;
    points: number;
}

export type UserModel = {
    nickName: string;
    isDisactivated: boolean;
    email: string;
    subscriptionPlan: SubscriptionPlan;
}

export type LoginModel = {
    nickName: string;
    password: string;
}

export type RegisterModel = {
    email: string;
    password: string;
    nickName: string;
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

export interface ElementClipModel {
    width: number;
    height: number;
}

export interface ElementModel {
    selector: string;
    clip: ElementClipModel;
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
    createdAt: string,
    state: ScreenshotState,
    type: ScreenshotType,
    title: string | null,
    description: string | null,
    categories: Category[],
}

export enum ScreenshotState
{
    New = 'New',
    Successful = 'Successful',
    Failed = 'Failed',
}

export type Paging = {
    page: number;
    pageSize: number;
    query?: string;
    searchScope: SearchScope;
    categoryIds?: string[];
};

export enum SearchScope
{
    None = 1 << 0,
    Title = 1 << 1,
    All = 1 << 2,
}

export type Category = {
    id: string;
    name: string;
    color: string;
};

export type PagedResult<T> = {
    items: T[];
    total: number;
};
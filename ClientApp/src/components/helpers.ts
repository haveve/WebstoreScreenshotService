import { ScreenshotState } from "../behavior/types";

export const setCookie = (name: string, value: string, days: number) => {
    const expires = new Date();
    expires.setTime(expires.getTime() + days * 24 * 60 * 60 * 1000);
    document.cookie = `${name}=${value};expires=${expires.toUTCString()};path=/`;
};

export const getCookie = (name: string) => {
    const match = document.cookie.match(new RegExp('(^| )' + name + '=([^;]+)'));
    return match ? match[2] : null;
};

export const getStateColor = (state: ScreenshotState) => {
    if (isSuccess(state))
        return "success";

    if (isFailed(state))
        return "success";

    return "warning";
};

export const isSuccess = (state: ScreenshotState) =>
    state === ScreenshotState.Successful;

export const isFailed = (state: ScreenshotState) =>
    state === ScreenshotState.Failed;

export const isLoading = (state: ScreenshotState) =>
    state === ScreenshotState.New;

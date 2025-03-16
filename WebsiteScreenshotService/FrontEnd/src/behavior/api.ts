import { ajax, AjaxConfig } from "rxjs/ajax";
import { trimStartCharacter } from "./utils";
import { catchError, map } from "rxjs";

const enum Methods {
    GET = "GET",
    POST = "POST",
    PUT = "PUT",
    DELETE = "DELETE"
}

export function AjaxObservable<T>(data: any, requestUrl: string, method: Methods = Methods.GET, withCredentials = false, responseType: XMLHttpRequestResponseType = "json") {
    const request: AjaxConfig = {
        url: requestUrl,
        method,
        headers: {
            'Content-Type': 'application/json',
        },
        withCredentials,
        responseType
    }

    if (method === Methods.POST || method === Methods.PUT)
        request["body"] = JSON.stringify(data)
    else {
        const quryParams = data ? `?${new URLSearchParams(data).toString()}` : "";
        request.url = `${requestUrl}${quryParams}`;
    }

    return ajax<T>(request)
}

type ScreenshotApiObservableResponse<T> = {
    response: T;
    error: string | null;
    status: number;
}

export function ScreenshotApiObservable<T>(data: any, path: string, method: Methods = Methods.GET, withCredentials = false, responseType: XMLHttpRequestResponseType = "json") {
    return AjaxObservable<T>(data, `${process.env.REACT_APP_BACK_URL}/${trimStartCharacter(path, "/")}`, method, withCredentials, responseType)
        .pipe(map(response => {
            var result: ScreenshotApiObservableResponse<T | null> = {
                error: null,
                response: response.response,
                status: response.status
            };
            return result;
        }), catchError(error => {
            return [{ response: null, error: error.xhr.statusText, status: 500 }];
        }));
}

export function PostScreenshotApiObservable<T>(body: any, path: string, withCredentials = false, responseType: XMLHttpRequestResponseType = "json") {
    return ScreenshotApiObservable<T>(body, path, Methods.POST, withCredentials, responseType);
}

export function GetScreenshotApiObservable<T>(path: string, withCredentials = false, quaryParams?: string[][], responseType: XMLHttpRequestResponseType = "json") {
    return ScreenshotApiObservable<T>(quaryParams, path, Methods.GET, withCredentials, responseType);
}
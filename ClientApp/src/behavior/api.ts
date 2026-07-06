import { ajax, AjaxConfig, AjaxResponse } from "rxjs/ajax";
import { trimStartCharacter } from "../utils/string";
import { catchError, finalize, map, Observable, of, shareReplay, switchMap } from "rxjs";

let accessToken: string | null = null;
let expires: Date | null = null;

export const setAccessToken = (
    token: string | null,
    exp: Date
) => {
    accessToken = token;
    expires = exp;
};

const TOKEN_REFRESH_BUFFER_MS = 2 * 60 * 1000;

let refreshInFlight$: Observable<void> | null = null;

function isTokenExpiring(): boolean {
    if (!accessToken || !expires)
        return true;

    return expires.getTime() - TOKEN_REFRESH_BUFFER_MS <= Date.now();
}

function refreshToken(): Observable<void> {
    if (refreshInFlight$)
        return refreshInFlight$;

    refreshInFlight$ = ajax<{
        accessToken: string;
        expires: string;
    }>({
        url: "/authorization/refresh",
        method: "POST",
        withCredentials: true,
        headers: {
            "Content-Type": "application/json"
        }
    }).pipe(
        map(r => {
            setAccessToken(
                r.response.accessToken,
                new Date(r.response.expires)
            );
        }),
        finalize(() => {
            refreshInFlight$ = null;
        }),
        shareReplay(1)
    );

    return refreshInFlight$;
}

function ensureValidToken(): Observable<void> {
    if (!isTokenExpiring())
        return of(void 0);

    return refreshToken();
}

function executeRequest<T>(
    request: AjaxConfig,
    requiresAuth: boolean
): Observable<AjaxResponse<T>> {

    const runRequest = () => {
        request.headers = {
            ...request.headers,
            ...(accessToken
                ? { Authorization: `Bearer ${accessToken}` }
                : {})
        };

        return ajax<T>(request);
    };

    if (!requiresAuth)
        return runRequest();

    return ensureValidToken().pipe(
        switchMap(() => runRequest())
    );
}

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

    return executeRequest<T>(
        request,
        withCredentials
    );
}

type ScreenshotApiObservableResponse<T> = {
    response: T;
    error: string | null;
    status: number;
}

export function ScreenshotApiObservable<T>(data: any, path: string, method: Methods = Methods.GET, withCredentials = false, responseType: XMLHttpRequestResponseType = "json") {
    return AjaxObservable<T>(data, `/${trimStartCharacter(path, "/")}`, method, withCredentials, responseType)
        .pipe(map(response => {
            debugger;
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

export function GetScreenshotApiObservable<T>(path: string, withCredentials = false, queryParams?: any, responseType: XMLHttpRequestResponseType = "json") {
    return ScreenshotApiObservable<T>(queryParams, path, Methods.GET, withCredentials, responseType);
}
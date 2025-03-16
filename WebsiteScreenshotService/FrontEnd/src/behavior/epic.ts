import { Epic, ofType, combineEpics } from "redux-observable";
import { map, exhaustMap } from "rxjs";
import { PayloadAction, createAction } from "@reduxjs/toolkit";
import { GetScreenshotApiObservable, PostScreenshotApiObservable } from "./api";
import { LoginModel, RegisterModel, ScreenshotOptionsModel, UserModel } from "./types";
import { setImage, setUser } from "./reducer";

export const getLoginAction = createAction<LoginModel>("login");
export const loginEpic: Epic<PayloadAction<LoginModel, "login">, any> = (action$) => action$.pipe(
    ofType("login"),
    map(action => action.payload),
    exhaustMap((payload) => {
        return PostScreenshotApiObservable<UserModel | null>(payload, "/Identity/Login", true).pipe(
            map(({ response: data, error }) => {
                return setUser({ data, error });
            })
        );
    })
);

export const getRegisterAction = createAction<RegisterModel>("register");
export const registerEpic: Epic<PayloadAction<RegisterModel, "register">, any> = (action$) => action$.pipe(
    ofType("register"),
    map(action => action.payload),
    exhaustMap((payload) => {
        return PostScreenshotApiObservable<UserModel | null>(payload, "/Identity/Register", true).pipe(
            map(({ response: data, error }) => {
                return setUser({ data, error });
            })
        );
    })
);

export const getLogoutAction = createAction("logout");
export const logoutEpic: Epic<PayloadAction<void, "logout">, any> = (action$) => action$.pipe(
    ofType("logout"),
    exhaustMap(_ => {
        return GetScreenshotApiObservable("/Identity/Logout", true).pipe(
            map(({ error }) => {
                return setUser({ data: null, error });
            })
        );
    })
);

export const getMakeScreenshotAction = createAction<ScreenshotOptionsModel>("makeScreenshot");
export const makeScreenshotEpic: Epic<PayloadAction<ScreenshotOptionsModel, "makeScreenshot">, any> = (action$) => action$.pipe(
    ofType("makeScreenshot"),
    map(action => action.payload),
    exhaustMap(payload => {
        return PostScreenshotApiObservable<Blob | null>(payload, "/MakeScreenshot", true, "blob").pipe(
            map(({ response: data, error }) => {
                return setImage({ data: data ? URL.createObjectURL(data) : data, error });
            })
        );
    })
);

export const getReceiveUserAction = createAction("getUser");
export const receiveUserEpic: Epic<PayloadAction<void, "getUser">, any> = (action$) => action$.pipe(
    ofType("getUser"),
    exhaustMap(_ => {
        return GetScreenshotApiObservable<UserModel | null>("/Identity/GetUserInfo", true).pipe(
            map(({ response: data, error }) => {
                return setUser({ data, error });
            })
        );
    })
);

const rootEpic: Epic = (action$, store$, dependencies) =>
    combineEpics<any>(loginEpic, registerEpic, logoutEpic, makeScreenshotEpic, receiveUserEpic)
        (action$, store$, dependencies);

export default rootEpic;
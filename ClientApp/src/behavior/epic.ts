import { Epic, ofType, combineEpics } from "redux-observable";
import { map, exhaustMap } from "rxjs";
import { PayloadAction, createAction } from "@reduxjs/toolkit";
import { GetScreenshotApiObservable, PostScreenshotApiObservable } from "./api";
import { LoginModel, RegisterModel, Screenshot, ScreenshotOptionsModel, UserModel } from "./types";
import { setScreenshot, setUser } from "./reducer";
import { mockUser } from "./mocks";

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
        return PostScreenshotApiObservable<Screenshot | null>(payload, "/MakeScreenshot", true, "blob").pipe(
            map(({ response: data, error }) => {
                return setScreenshot({ data, error });
            })
        );
    })
);

export const getScreenshotAction = createAction<string>("getScreenshot");
export const getScreenshotEpic: Epic<PayloadAction<string, "getScreenshot">, any> = (action$) => action$.pipe(
    ofType("getScreenshot"),
    map(action => action.payload),
    exhaustMap(id => {
        return GetScreenshotApiObservable<Screenshot | null>("/GetScreenshot", true, { "id": id }).pipe(
            map(({ response: data, error }) => {
                return setScreenshot({ data, error });
            })
        );
    })
);

export const getReceiveUserAction = createAction("getUser");
export const receiveUserEpic: Epic<PayloadAction<void, "getUser">, any> = (action$) => action$.pipe(
    ofType("getUser"),
    map(_ => setUser({ data: mockUser, error: null })),
    // exhaustMap(_ => {
    //     return GetScreenshotApiObservable<UserModel | null>("/Identity/GetUserInfo", true).pipe(
    //         map(({ response: data, error }) => {
    //             return setUser({ data, error });
    //         })
    //     );
    // })
);

const rootEpic: Epic = (action$, store$, dependencies) =>
    combineEpics<any>(loginEpic, registerEpic, logoutEpic, makeScreenshotEpic, getScreenshotEpic, receiveUserEpic)
        (action$, store$, dependencies);

export default rootEpic;
import { Epic, ofType, combineEpics } from "redux-observable";
import { map, exhaustMap, switchMap, from, of, delay, mergeMap } from "rxjs";
import { PayloadAction, createAction } from "@reduxjs/toolkit";
import { GetScreenshotApiObservable, PostScreenshotApiObservable, setAccessToken } from "./api";
import { Category, LoginModel, Paging, RegisterModel, Screenshot, ScreenshotOptionsModel, ScreenshotState, UserModel } from "./types";
import { cancelSubscription, setCategories, setScreenshot, setScreenshots, setUser } from "./reducer";
import { formatQueryPaging, setPagingQuery } from "./utils/search";


type LoginResponse = {
    accessToken: string,
    expires: string,
    userData: UserModel
}

export const getLoginAction = createAction<LoginModel>("login");
export const loginEpic: Epic<PayloadAction<LoginModel, "login">, any> = (action$) => action$.pipe(
    ofType("login"),
    map(action => action.payload),
    exhaustMap((payload) => {
        return PostScreenshotApiObservable<LoginResponse | null>(payload, "/authorization/login", false).pipe(
            map(({ response: data, error }) => {
                data && setAccessToken(data.accessToken, new Date(data.expires));
                return setUser({ data: data?.userData, error });
            })
        );
    })
);

type ListResult = {
    items: Screenshot[],
    totalCount: number
}

export const getScreenshotsAction = createAction<Paging | undefined>("getScreenshots");
export const getScreenshotsEpic: Epic<
    PayloadAction<Paging | undefined, "getScreenshots">,
    any
> = (action$) =>
        action$.pipe(
            ofType("getScreenshots"),
            map((a) => {
                var paging = a.payload ?? formatQueryPaging();
                setPagingQuery(paging);

                if (!paging.categoryIds?.length)
                    delete paging.categoryIds;

                return paging;
            }),
            switchMap((paging) =>
                GetScreenshotApiObservable<ListResult>(
                    "/screenshots/getScreenshots",
                    true,
                    { ...paging }
                ).pipe(
                    map(({ response: data, error }) =>
                        setScreenshots({ data: data ? { items: data.items, total: data.totalCount, page: paging.page, pageSize: paging.pageSize } : null, error })
                    )
                )
            )
        );

type UpdateScreenshot = {
    id: string;
    title?: string;
    description?: string;
    categories?: string[];
};

export const updateScreenshotAction = createAction<UpdateScreenshot>("updateScreenshot");
export const updateScreenshotEpic: Epic = (action$) =>
    action$.pipe(
        ofType("updateScreenshot"),
        exhaustMap(({ payload }) =>
            PostScreenshotApiObservable<Screenshot>(
                payload,
                "/screenshots/updateScreenshot",
                true
            ).pipe(
                mergeMap(({ response, error }) => [
                    getScreenshotsAction()
                ])
            )
        )
    );

type CreateCategory = {
    name: string;
    color: string;
}

export const createCategoryAction = createAction<CreateCategory>("createCategory");
export const createCategoryEpic: Epic = (action$) =>
    action$.pipe(
        ofType("createCategory"),
        exhaustMap(({ payload }) =>
            PostScreenshotApiObservable<Category>(
                payload,
                "/categories/createCategory",
                true
            ).pipe(
                mergeMap(({ response, error }) => [
                    getCategoriesAction()
                ])
            )
        )
    );

export const getCategoriesAction = createAction("getCategories");
export const getCategoriesEpic: Epic = (action$) =>
    action$.pipe(
        ofType("getCategories"),
        exhaustMap(() =>
            GetScreenshotApiObservable<Category[]>(
                "/categories/getCategories",
                true
            ).pipe(
                map(({ response: data, error }) =>
                    setCategories({ data, error })
                )
            )
        )
    );

export const getRegisterAction = createAction<RegisterModel>("register");
export const registerEpic: Epic<PayloadAction<RegisterModel, "register">, any> = (action$) => action$.pipe(
    ofType("register"),
    map(action => action.payload),
    exhaustMap((payload) => {
        debugger;
        return PostScreenshotApiObservable<UserModel | null>(payload, "/authorization/register", false).pipe(
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
        return GetScreenshotApiObservable("/identity/logout", true).pipe(
            map(({ error }) => {
                return setUser({ data: null, error });
            })
        );
    })
);

const randomId = () => Math.random().toString(36).substring(2, 10);

const now = () => new Date().toISOString();

export const getMakeScreenshotAction = createAction<ScreenshotOptionsModel>("makeScreenshot");
export const makeScreenshotEpic: Epic<PayloadAction<ScreenshotOptionsModel, "makeScreenshot">, any> = (action$) => action$.pipe(
    ofType("makeScreenshot"),
    map(action => action.payload),
    exhaustMap(payload => {
        return PostScreenshotApiObservable<Screenshot | null>(payload, "screenshots/makeScreenshot", true).pipe(
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
        return GetScreenshotApiObservable<Screenshot | null>("screenshots/getScreenshot", true, { "id": id }).pipe(
            map(({ response: data, error }) => {
                return setScreenshot({ data, error });
            })
        );
    }),
);

export const cancelSubscriptionAction = createAction("cancelSubscription");
export const cancelSubscriptionEpic: Epic<PayloadAction<void, "cancelSubscription">, any> = (action$) => action$.pipe(
    ofType("cancelSubscription"),
    map(_ => cancelSubscription({ data: undefined, error: null })),
);

export const getReceiveUserAction = createAction("getUser");
export const receiveUserEpic: Epic<PayloadAction<void, "getUser">, any> = (action$) => action$.pipe(
    ofType("getUser"),
    exhaustMap(_ => {
        return GetScreenshotApiObservable<UserModel | null>("/identity/getUserInfo", true).pipe(
            map(({ response: data, error }) => {
                return setUser({ data, error });
            })
        );
    })
);

const rootEpic: Epic = (action$, store$, dependencies) =>
    combineEpics<any>(loginEpic, registerEpic, logoutEpic, updateScreenshotEpic, makeScreenshotEpic, createCategoryEpic, getScreenshotEpic, receiveUserEpic, getScreenshotsEpic, getCategoriesEpic)
        (action$, store$, dependencies);

export default rootEpic;
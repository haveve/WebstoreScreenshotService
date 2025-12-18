import { SubscriptionType, UserModel } from "./types";

export const mockUser: UserModel = {
    name: 'John',
    surname: 'Doe',
    email: 'john.doe@example.com',
    subscriptionPlan: {
        type: SubscriptionType.Regular,
        screenshotLeft: 25,
    },
};
import { Duration, SubscriptionType, UserModel } from "./types";

export const mockUser: UserModel = {
    nickName: 'super-user',
    isDisactivated: false,
    email: 'ipz224_pis@student.ztu.edu.ua',
    subscriptionPlan: {
        type: SubscriptionType.Advanced,
        duration: Duration.Yearly,
        points: 50_000,
    },
};
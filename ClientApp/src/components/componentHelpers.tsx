import { Typography } from "@mui/material";
import { SubscriptionType } from "../behavior/types";
import { TFunction } from "react-i18next";

export function getPlanDescription(plan: SubscriptionType, t: TFunction<"translations", undefined>) {
    switch (plan) {
        case SubscriptionType.Regular:
            return <Typography component={"span"} color="primary">{t('MyAccount.regularPlan')}</Typography>;
        case SubscriptionType.Pro:
            return <Typography component={"span"} color="info">{t('MyAccount.proPlan')}</Typography>;
        case SubscriptionType.Advanced:
            return <Typography component={"span"} color="error">{t('MyAccount.advancedPlan')}</Typography>;
        default:
            return t('MyAccount.unknownPlan');
    }
}

export function getPointsDescription(points: number) {
    if (points > 300)
        return <Typography component={"span"} color="success">{points}</Typography>;

    if (points > 100)
        return <Typography component={"span"} color="warning">{points}</Typography>;

    return <Typography component={"span"} color="error">{points}</Typography>;
}
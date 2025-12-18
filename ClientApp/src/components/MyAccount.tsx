import { useEffect } from "react";
import { useAppSelector } from "../behavior/rootReducer";
import { useDispatch } from "react-redux";
import { getReceiveUserAction } from "../behavior/epic";
import { SubscriptionType } from "../behavior/types";
import { useTranslation } from 'react-i18next';
import {
    Container,
    Card,
    Typography,
    Box,
    Stack,
    Link
} from '@mui/material';

const MyAccount = () => {
    const user = useAppSelector((state) => state.user!);
    const loaded = useAppSelector((state) => state.loaded);
    const dispatch = useDispatch();
    const { t } = useTranslation();

    useEffect(() => {
        dispatch(getReceiveUserAction());
    }, [dispatch]);

    if (!loaded)
        return null;

    return (
        <Container maxWidth="sm" sx={{ mt: 4 }}>
            <Card>
                <Box sx={{ p: 3 }}>
                    <Typography variant="h4" gutterBottom>
                        {t('MyAccount.myAccount')}
                    </Typography>
                    <Stack spacing={2}>
                        <Typography>
                            <strong>{t('MyAccount.email')}</strong>А{user.email}
                        </Typography>
                        <Typography>
                            <strong>{t('MyAccount.firstName')}</strong> {user.name}
                        </Typography>
                        <Typography>
                            <strong>{t('MyAccount.lastName')}</strong> {user.surname}
                        </Typography>
                        <Typography>
                            <strong>{t('MyAccount.screenshotsLeft')}</strong> {user.subscriptionPlan.screenshotLeft}
                            <br />
                            <strong>{t('MyAccount.plan')}</strong> {getPlanDescription(user.subscriptionPlan.type, t)}
                        </Typography>
                        <Typography>
                            {t('MyAccount.gdprContact')} <b>{t('MyAccount.gdpr')}</b>{t('MyAccount.contactUs')}{" "}
                            <Link href="/privacy-policy">{t('MyAccount.contactInformation')}</Link>
                        </Typography>
                    </Stack>
                </Box>
            </Card>
        </Container>
    );
};

function getPlanDescription(plan: SubscriptionType, t: any) {
    switch (plan) {
        case SubscriptionType.Regular:
            return t('MyAccount.regularPlan');
        default:
            return t('MyAccount.unknownPlan');
    }
}

export default MyAccount;

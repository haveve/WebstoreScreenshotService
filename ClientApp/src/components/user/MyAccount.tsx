import { useEffect, useState } from "react";
import { useAppSelector } from "../../behavior/rootReducer";
import { useDispatch } from "react-redux";
import {
    getReceiveUserAction,
    cancelSubscriptionAction
} from "../../behavior/epic";
import { useTranslation } from 'react-i18next';

import {
    Container,
    Card,
    Typography,
    Box,
    Stack,
    Link,
    Chip,
    Button,
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    Alert
} from '@mui/material';

import { getPlanDescription, getPointsDescription } from "../componentHelpers";
import { SubscriptionType } from "../../behavior/types";
import { useNavigate } from "react-router-dom";

const MyAccount = () => {
    const user = useAppSelector((state) => state.basic.user!);
    const loaded = useAppSelector((state) => state.basic.loaded);

    const navigate = useNavigate();
    const dispatch = useDispatch();
    const { t } = useTranslation();

    const [cancelOpen, setCancelOpen] = useState(false);

    useEffect(() => {
        dispatch(getReceiveUserAction());
    }, [dispatch]);

    if (!loaded)
        return null;

    const hasPaidSubscription =
        user.subscriptionPlan?.type &&
        user.subscriptionPlan.type !== SubscriptionType.Regular;

    const handleCancelSubscription = () => {
        dispatch(cancelSubscriptionAction());
        setCancelOpen(false);
    };

    return (
        <Container maxWidth="sm" sx={{ mt: 4 }}>
            <Card
                sx={{
                    borderRadius: 4,
                    boxShadow: 3
                }}
            >
                <Box sx={{ p: 4 }}>
                    <Typography
                        variant="h4"
                        gutterBottom
                        fontWeight={700}
                    >
                        {t('MyAccount.myAccount')}
                    </Typography>

                    <Stack spacing={2.5}>

                        <Typography>
                            <strong>{t('MyAccount.email')}</strong> {user.email}
                        </Typography>

                        <Typography>
                            <strong>{t('MyAccount.nickName')}</strong> {user.nickName}
                        </Typography>

                        <Typography>
                            <strong>{t('MyAccount.isActive')}</strong>{" "}
                            {!user.isDisactivated ? (
                                <Chip
                                    size="small"
                                    color="success"
                                    label="АКТИВНИЙ"
                                />
                            ) : (
                                <Chip
                                    size="small"
                                    color="error"
                                    label="ВИМКНЕНО"
                                />
                            )}
                        </Typography>

                        <Typography>
                            <strong>{t('MyAccount.points')}</strong>{" "}
                            {getPointsDescription(user.subscriptionPlan.points)}
                        </Typography>

                        <Box>
                            <Typography mb={1}>
                                <strong>{t('MyAccount.plan')}</strong>{" "}
                                {getPlanDescription(user.subscriptionPlan.type, t)}
                            </Typography>

                            <Stack
                                direction="row"
                                spacing={1.5}
                                flexWrap="wrap"
                            >
                                <Button
                                    color="secondary"
                                    variant="outlined"
                                    size="small"
                                    onClick={() => navigate("/#subscriptions")}
                                >
                                    {t("MyAccount.changePlan")}
                                </Button>

                                {hasPaidSubscription && (
                                    <Button
                                        color="error"
                                        variant="outlined"
                                        size="small"
                                        onClick={() => setCancelOpen(true)}
                                    >
                                        Скасувати підписку
                                    </Button>
                                )}
                            </Stack>
                        </Box>

                        <Typography>
                            {t('MyAccount.gdprContact')}{" "}
                            <b>{t('MyAccount.gdpr')}</b>
                            {t('MyAccount.contactUs')}{" "}
                            <Link href="/privacy-policy">
                                {t('MyAccount.contactInformation')}
                            </Link>
                        </Typography>
                    </Stack>
                </Box>
            </Card>

            {/* CONFIRMATION DIALOG */}
            <Dialog
                open={cancelOpen}
                onClose={() => setCancelOpen(false)}
                maxWidth="xs"
                fullWidth
            >
                <DialogTitle fontWeight={700}>
                    Скасування підписки
                </DialogTitle>

                <DialogContent>
                    <Typography mb={1}>
                        Ви впевнені, що хочете скасувати підписку?
                    </Typography>

                    <Alert severity="info" variant="outlined">
                        Після скасування підписка залишатиметься активною
                        до завершення поточного оплаченого періоду.
                    </Alert>
                </DialogContent>

                <DialogActions sx={{ px: 3, pb: 3 }}>
                    <Button
                        onClick={() => setCancelOpen(false)}
                    >
                        Закрити
                    </Button>

                    <Button
                        color="error"
                        variant="contained"
                        onClick={handleCancelSubscription}
                    >
                        Скасувати підписку
                    </Button>
                </DialogActions>
            </Dialog>
        </Container>
    );
};

export default MyAccount;
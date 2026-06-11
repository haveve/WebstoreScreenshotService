import {
    Dialog,
    Typography,
    Button,
    Box,
    Paper,
    Stack,
    Divider,
    Chip,
    CircularProgress
} from "@mui/material";
import { useDispatch } from "react-redux";
import { useState } from "react";
import { createSubscriptionCheckout } from "../../behavior/basket/epic";
import CheckCircleIcon from "@mui/icons-material/CheckCircle";
import { Duration, SubscriptionType } from "../../behavior/types";
import { StripeCheckout } from "./StripeCheckout";

type Props = {
    onClose: () => void;
    open: boolean;
    plan: SubscriptionType;
    duration: Duration;
    price: number;
    benefits?: string[];
    isPopular?: boolean;
};

export const SubscriptionCheckoutModal = ({
    open,
    onClose,
    plan,
    price,
    duration,
    benefits = [],
    isPopular
}: Props) => {
    const dispatch = useDispatch();
    const [loading, setLoading] = useState(false);

    const pay = () => {
        setLoading(true);

        setTimeout(() => {
            dispatch(createSubscriptionCheckout({ plan, duration, price }));
            setLoading(false);
            onClose();
        }, 1200);
    };

    return (
        <Dialog open={open} onClose={onClose} maxWidth="xs" fullWidth>
            <Box sx={{ p: 3 }}>

                {/* HEADER */}
                <Box sx={{ mb: 2, textAlign: "center" }}>
                    <Typography variant="h6" fontWeight={700}>
                        Підтвердження підписки
                    </Typography>

                    <Typography variant="body2" color="text.secondary">
                        Безпечна оплата
                    </Typography>
                </Box>

                {/* CARD */}
                <Paper
                    elevation={3}
                    sx={{
                        mb: 3,
                        p: 3,
                        borderRadius: 3,
                        border: isPopular ? "2px solid" : "1px solid transparent",
                        borderColor: isPopular ? "primary.main" : "transparent",
                        position: "relative"
                    }}
                >
                    {isPopular && (
                        <Chip
                            label="Найпопулярніший"
                            color="primary"
                            size="small"
                            sx={{ position: "absolute", top: 12, right: 12 }}
                        />
                    )}

                    {/* PLAN */}
                    <Typography variant="h5" fontWeight={700}>
                        {plan === SubscriptionType.Pro ? "Pro" : "Advanced"}
                    </Typography>

                    {/* PRICE */}
                    <Box sx={{ mt: 2, mb: 2 }}>
                        <Typography variant="h3" fontWeight={800}>
                            ${price}
                        </Typography>

                        <Typography variant="caption" color="text.secondary">
                            за {duration === Duration.Monthly ? "місяць" : "рік"}
                        </Typography>
                    </Box>

                    <Divider sx={{ my: 2 }} />

                    {/* BENEFITS PREVIEW */}
                    <Stack spacing={1}>
                        {benefits.map((b, idx) => (
                            <Box
                                key={idx}
                                sx={{
                                    display: "flex",
                                    alignItems: "center",
                                    gap: 1
                                }}
                            >
                                <CheckCircleIcon color="primary" fontSize="small" />
                                <Typography variant="body2">{b}</Typography>
                            </Box>
                        ))}
                    </Stack>
                </Paper>

                <StripeCheckout amount={price} onSuccess={pay} />
            </Box>
        </Dialog>
    );
};
import {
    Dialog,
    Typography,
    Stack,
    Box,
    Paper,
    Divider,
    IconButton
} from "@mui/material";
import DeleteIcon from "@mui/icons-material/Delete";
import ShoppingCartIcon from "@mui/icons-material/ShoppingCart";
import { useDispatch } from "react-redux";
import { toggleBasket, removeItem } from "../../behavior/basket/reducer";
import { createBasketCheckout } from "../../behavior/basket/epic";
import { useAppSelector } from "../../behavior/rootReducer";
import { StripeCheckout } from "./StripeCheckout";

export const BasketModal = () => {
    const dispatch = useDispatch();
    const basket = useAppSelector((s) => s.basket);

    const total = basket.items.reduce((sum, i) => sum + i.price, 0);

    return (
        <Dialog
            open={basket.open}
            onClose={() => dispatch(toggleBasket())}
            maxWidth="xs"
            fullWidth
        >
            <Box sx={{ p: 3 }}>

                {/* HEADER */}
                <Box sx={{ textAlign: "center", mb: 2 }}>
                    <ShoppingCartIcon color="primary" />
                    <Typography variant="h6" fontWeight={700}>
                        Ваш кошик
                    </Typography>

                    <Typography variant="body2" color="text.secondary">
                        Перегляньте ваші бали перед оформленням замовлення
                    </Typography>
                </Box>

                {/* EMPTY STATE */}
                {basket.items.length === 0 ? (
                    <Paper
                        variant="outlined"
                        sx={{
                            p: 3,
                            textAlign: "center",
                            borderRadius: 3
                        }}
                    >
                        <Typography variant="body2" color="text.secondary">
                            Ваш кошик порожній
                        </Typography>
                    </Paper>
                ) : (
                    <>
                        {/* ITEMS */}
                        <Stack spacing={1.5}>
                            {basket.items.map((i) => (
                                <Paper
                                    key={i.id}
                                    variant="outlined"
                                    sx={{
                                        p: 2,
                                        borderRadius: 2,
                                        display: "flex",
                                        justifyContent: "space-between",
                                        alignItems: "center"
                                    }}
                                >
                                    {/* LEFT */}
                                    <Box>
                                        <Typography fontWeight={600}>
                                            {i.amount.toLocaleString()} балів
                                        </Typography>

                                        <Typography variant="caption" color="text.secondary">
                                            ${(i.price).toFixed(2)}
                                        </Typography>
                                    </Box>

                                    {/* RIGHT ACTION */}
                                    <IconButton
                                        size="small"
                                        onClick={() => dispatch(removeItem(i.id))}
                                    >
                                        <DeleteIcon fontSize="small" />
                                    </IconButton>
                                </Paper>
                            ))}
                        </Stack>

                        {/* TOTAL */}
                        <Divider sx={{ my: 2 }} />

                        <Box
                            sx={{
                                display: "flex",
                                justifyContent: "space-between",
                                alignItems: "center",
                                mb: 2
                            }}
                        >
                            <Typography variant="subtitle1" fontWeight={700}>
                                Разом
                            </Typography>

                            <Typography variant="h6" fontWeight={800}>
                                ${total.toFixed(2)}
                            </Typography>
                        </Box>

                        <Divider sx={{ my: 2 }} />

                        <StripeCheckout
                            amount={total}
                            onSuccess={() => dispatch(createBasketCheckout())}
                        />
                    </>
                )}
            </Box>
        </Dialog>
    );
};
import { Dashboard as DashboardIcon, ReceiptLong as ReceiptLongIcon, ShoppingBag as ShoppingBagIcon, TokenOutlined as TokenIcon } from "@mui/icons-material";
import { Box, List, ListItemButton, ListItemText, Paper } from "@mui/material";
import { NavLink, Outlet } from "react-router-dom";

const navItemStyle = {
    borderRadius: 2,
    px: 2,
    py: 1.2,
    mb: 0.5,
    transition: "all 0.2s ease",
    "&:hover": {
        backgroundColor: "action.hover",
        transform: "translateX(2px)",
    },
};


export const AccountLayout = () => {
    return (
        <Box sx={{ display: "flex", gap: 1 }}>

            {/* SIDEBAR */}
            <Paper
                elevation={0}
                sx={{
                    width: 260,
                    p: 2,
                    borderRadius: 3,
                    border: "1px solid",
                    borderColor: "divider",
                    position: "sticky",
                    top: 20,
                    backgroundColor: "background.paper",
                }}
            >
                <List sx={{ p: 0 }}>
                    {/* OVERVIEW */}
                    <ListItemButton
                        component={NavLink}
                        to="/my-account"
                        end
                        sx={{
                            ...navItemStyle,
                            "&.active": {
                                backgroundColor: "primary.main",
                                color: "white",
                                "& .MuiListItemText-primary": {
                                    fontWeight: 700,
                                },
                            },
                        }}
                    >
                        <DashboardIcon sx={{ mr: 1.5, fontSize: 20 }} />
                        <ListItemText primary="Мій акаунт" />
                    </ListItemButton>

                    {/* TRANSACTIONS */}
                    <ListItemButton
                        component={NavLink}
                        to="/my-account/transactions"
                        sx={{
                            ...navItemStyle,
                            "&.active": {
                                backgroundColor: "primary.main",
                                color: "white",
                                "& .MuiListItemText-primary": {
                                    fontWeight: 700,
                                },
                            },
                        }}
                    >
                        <ReceiptLongIcon sx={{ mr: 1.5, fontSize: 20 }} />
                        <ListItemText primary="Транзакції" />
                    </ListItemButton>

                    <ListItemButton
                        component={NavLink}
                        to="/my-account/orders"
                        sx={{
                            ...navItemStyle,
                            "&.active": {
                                backgroundColor: "primary.main",
                                color: "white",
                                "& .MuiListItemText-primary": {
                                    fontWeight: 700,
                                },
                            },
                        }}
                    >
                        <ShoppingBagIcon sx={{ mr: 1.5, fontSize: 20 }} />
                        <ListItemText primary="Іторія замовлень" />
                    </ListItemButton>

                    <ListItemButton
                        component={NavLink}
                        to="/my-account/token-management"
                        sx={{
                            ...navItemStyle,
                            "&.active": {
                                backgroundColor: "primary.main",
                                color: "white",
                                "& .MuiListItemText-primary": {
                                    fontWeight: 700,
                                },
                            },
                        }}
                    >
                        <TokenIcon sx={{ mr: 1.5, fontSize: 20 }} />
                        <ListItemText primary="Керування API токенами" />
                    </ListItemButton>

                </List>
            </Paper>

            {/* CONTENT */}
            <Box sx={{ flex: 1 }}>
                <Outlet />
            </Box>
        </Box>
    );
};
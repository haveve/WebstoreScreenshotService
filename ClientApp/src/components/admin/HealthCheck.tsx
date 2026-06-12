import React, { useEffect, useState } from "react";
import {
    Box,
    Typography,
    Chip,
    Accordion,
    AccordionSummary,
    AccordionDetails,
    Paper,
    Stack,
    LinearProgress,
    Switch,
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    Button,
    useTheme,
} from "@mui/material";

type HealthStatus = "healthy" | "degraded" | "down" | "loading";

interface ServiceNode {
    id: string;
    name: string;
    status: HealthStatus;
    message: string;
    children?: ServiceNode[];
}

/* ---------------- MOCK ---------------- */

const mockTree: ServiceNode = {
    id: "api-gateway",
    name: "API Gateway (ASP.NET)",
    status: "healthy",
    message: "Усі ендпоінти шлюзу працюють",
    children: [
        {
            id: "payment",
            name: "Stripe Payment Service",
            status: "healthy",
            message: "Обробка платежів працює нормально",
        },
        {
            id: "rabbitmq",
            name: "RabbitMQ Message Broker",
            status: "healthy",
            message: "Черга працює справно",
            children: [
                {
                    id: "q-regular",
                    name: "Regular Queue",
                    status: "healthy",
                    message: "Стабільна пропускна здатність",
                },
                {
                    id: "q-pro",
                    name: "Pro Queue",
                    status: "healthy",
                    message: "Стабільна пропускна здатність",
                },
                {
                    id: "q-advanced",
                    name: "Advanced Queue",
                    status: "healthy",
                    message: "Стабільна пропускна здатність",
                },
            ],
        },
        {
            id: "postgres",
            name: "PostgreSQL Database",
            status: "healthy",
            message: "Читання/запис OK",
        },
        {
            id: "blob",
            name: "Azure Blob Storage",
            status: "healthy",
            message: "Сховище скриншотів працює",
        },
    ],
};

/* ---------------- STATUS ---------------- */

const statusRank: Record<HealthStatus, number> = {
    healthy: 0,
    loading: 1,
    degraded: 2,
    down: 3,
};

const worst = (a: HealthStatus, b: HealthStatus): HealthStatus =>
    statusRank[b] > statusRank[a] ? b : a;

/* ---------------- DEP STATUS ---------------- */

const computeDependencyStatus = (node: ServiceNode): HealthStatus => {
    if (!node.children?.length) return node.status;

    const childWorst = node.children.reduce(
        (acc, c) => worst(acc, computeDependencyStatus(c)),
        "healthy" as HealthStatus
    );

    return worst(node.status, childWorst);
};

/* ---------------- STYLES ---------------- */

const getStyles = (status: HealthStatus, theme: any) => {
    switch (status) {
        case "healthy":
            return {
                border: `1px solid ${theme.palette.success.light}`,
                accent: theme.palette.success.main,
                bg: "rgba(46, 125, 50, 0.06)",
            };
        case "degraded":
            return {
                border: `1px solid ${theme.palette.warning.light}`,
                accent: theme.palette.warning.main,
                bg: "rgba(237, 108, 2, 0.06)",
            };
        case "down":
            return {
                border: `1px solid ${theme.palette.error.light}`,
                accent: theme.palette.error.main,
                bg: "rgba(211, 47, 47, 0.06)",
            };
        default:
            return {
                border: `1px solid ${theme.palette.grey[300]}`,
                accent: theme.palette.grey[500],
                bg: theme.palette.grey[50],
            };
    }
};

/* ---------------- NODE RENDER ---------------- */

const renderNode = (node: ServiceNode, level = 0) => {
    // eslint-disable-next-line react-hooks/rules-of-hooks
    const theme = useTheme();
    const styles = getStyles(node.status, theme);

    const hasChildren = !!node.children?.length;
    const depStatus = computeDependencyStatus(node);
    const showDep = hasChildren && depStatus !== node.status;

    return (
        <Box key={node.id} sx={{ ml: level * 2, mb: 1 }}>
            <Paper
                elevation={0}
                sx={{
                    border: styles.border,
                    borderLeft: `5px solid ${styles.accent}`,
                    backgroundColor: styles.bg,
                    borderRadius: 2,
                    overflow: "hidden",
                    transition: "all 0.2s ease",
                    "&:hover": {
                        transform: "translateY(-1px)",
                        boxShadow: "0 6px 18px rgba(0,0,0,0.08)",
                    },
                }}
            >
                <Accordion
                    disableGutters
                    elevation={0}
                    sx={{
                        background: "transparent",
                        "&:before": { display: "none" },
                    }}
                >
                    <AccordionSummary>
                        <Stack direction="row" alignItems="center" spacing={2} sx={{ width: "100%" }}>
                            <Typography sx={{ flexGrow: 1, fontWeight: 600 }}>
                                {node.name}
                            </Typography>

                            {showDep && (
                                <Chip
                                    size="small"
                                    label={`ЗАЛЕЖНІСТЬ ${depStatus.toUpperCase()}`}
                                    sx={{
                                        fontWeight: 600,
                                        fontSize: "0.65rem",
                                        backgroundColor:
                                            depStatus === "down"
                                                ? theme.palette.error.main
                                                : depStatus === "degraded"
                                                ? theme.palette.warning.main
                                                : theme.palette.grey[500],
                                        color: "#fff",
                                    }}
                                />
                            )}

                            <Chip
                                size="small"
                                label={node.status.toUpperCase()}
                                sx={{
                                    fontWeight: 600,
                                    fontSize: "0.7rem",
                                    backgroundColor: styles.accent,
                                    color: "#fff",
                                }}
                            />
                        </Stack>
                    </AccordionSummary>

                    <AccordionDetails>
                        <Typography variant="body2" sx={{ mb: 1.5 }}>
                            {node.message}
                        </Typography>

                        {node.children?.map((c) => renderNode(c, level + 1))}
                    </AccordionDetails>
                </Accordion>
            </Paper>
        </Box>
    );
};

/* ---------------- MAIN ---------------- */

export default function HealthDependencyTree() {
    const [loading, setLoading] = useState(true);
    const [data, setData] = useState<ServiceNode | null>(null);

    const [maintenanceMode, setMaintenanceMode] = useState(false);
    const [pendingValue, setPendingValue] = useState<boolean | null>(null);
    const [confirmOpen, setConfirmOpen] = useState(false);

    useEffect(() => {
        return recheck();
    }, []);

    const recheck = () => {
        setLoading(true);
        const t = setTimeout(() => {
            setData(mockTree);
            setLoading(false);
        }, 1000);

        return () => clearTimeout(t);
    };

    const handleToggle = (value: boolean) => {
        setPendingValue(value);
        setConfirmOpen(true);
    };

    const confirmChange = () => {
        setMaintenanceMode(!!pendingValue);
        setConfirmOpen(false);
        setPendingValue(null);
    };

    return (
        <Box p={3} sx={{ background: "#fafafa", minHeight: "100%" }}>
            {/* HEADER */}
            <Stack direction="row" justifyContent="space-between" alignItems="center">
                <Typography variant="h5" sx={{ fontWeight: 700 }}>
                    Інформація про життєвий цикл сервісу Screenshot
                </Typography>
            </Stack>

            {/* Maintenance toggle */}
            <Stack direction="row" alignItems="center" spacing={1} my={1}>
                <Typography variant="body2" color="text.secondary">
                    Режим обслуговування
                </Typography>

                <Switch
                    checked={maintenanceMode}
                    onChange={(e) => handleToggle(e.target.checked)}
                />
            </Stack>

            <Typography variant="h6" sx={{ fontWeight: 700 }} mb={1}>
                Перевірка стану системи{" "}
                <Button onClick={recheck} variant="contained" size="small">
                    Перевірити знову
                </Button>
            </Typography>

            {loading || !data ? (
                <Box p={3}>
                    <Typography variant="h6" mb={2} fontWeight={600}>
                        Перевірка стану системи...
                    </Typography>
                    <LinearProgress />
                </Box>
            ) : (
                <>
                    {data && renderNode(data)}

                    {/* CONFIRMATION */}
                    <Dialog open={confirmOpen} onClose={() => setConfirmOpen(false)}>
                        <DialogTitle>Підтвердження зміни</DialogTitle>
                        <DialogContent>
                            <Typography>
                                {pendingValue
                                    ? "Увімкнути режим обслуговування? (фронтенд/бекенд буде вимкнено для користувачів)"
                                    : "Вимкнути режим обслуговування та відновити роботу системи?"}
                            </Typography>
                        </DialogContent>
                        <DialogActions>
                            <Button onClick={() => setConfirmOpen(false)}>Скасувати</Button>
                            <Button variant="contained" onClick={confirmChange}>
                                Підтвердити
                            </Button>
                        </DialogActions>
                    </Dialog>
                </>
            )}
        </Box>
    );
}
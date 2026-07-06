import React, { useMemo, useState } from "react";
import {
    Box,
    Card,
    CardContent,
    Grid,
    Typography,
    Table,
    TableHead,
    TableRow,
    TableCell,
    TableBody,
    Paper,
    Stack,
    Select,
    MenuItem,
    FormControl,
    InputLabel,
    TextField,
    Button,
} from "@mui/material";

const MS_PER_DAY = 24 * 60 * 60 * 1000;

const toDate = (d: string) => new Date(d);

const toISO = (d: Date) => d.toISOString().slice(0, 10);

const addDays = (date: Date, days: number) =>
    new Date(date.getTime() + days * MS_PER_DAY);

const today = new Date();

const MIN_DATE = addDays(today, -365 * 5);
const MAX_DATE = today;

const clampDate = (date: Date, min: Date, max: Date) => {
    if (date < min) return min;
    if (date > max) return max;
    return date;
};

/* ---------------- TYPES ---------------- */

type Plan = "Free" | "Pro" | "Advanced";

type PlanRow = {
    plan: Plan;
    users: number;
    revenue: number;
    orderProfit: number;
};

/* ---------------- MOCK DATA ---------------- */

const basePlans: PlanRow[] = [
    { plan: "Free", users: 20, revenue: 0, orderProfit: 40 },
    { plan: "Pro", users: 5, revenue: 31.96, orderProfit: 100 },
    { plan: "Advanced", users: 4, revenue: 119.96, orderProfit: 200 },
];

/* ---------------- UI ---------------- */

function StatCard({
    title,
    value,
}: {
    title: string;
    value: string | number;
}) {
    return (
        <Card sx={{ borderRadius: 3 }}>
            <CardContent>
                <Typography color="text.secondary">
                    {title}
                </Typography>
                <Typography variant="h4" fontWeight={700}>
                    {value}
                </Typography>
            </CardContent>
        </Card>
    );
}

/* ---------------- DONUT CHART ---------------- */

function DonutChart({
    data,
    valueKey,
    title,
}: {
    data: PlanRow[];
    valueKey: "profit" | "mrr" | "orderProfit";
    title: string;
}) {
    const selectValue = (row: PlanRow) => {
        switch (valueKey) {
            case "mrr":
                return row.revenue;
            case "orderProfit":
                return row.orderProfit;
            case "profit":
                return row.revenue + row.orderProfit;
        }
    };

    const total = data.reduce((a, b) => a + selectValue(b), 0);

    const size = 220;
    const radius = 70;
    const stroke = 22;
    const center = size / 2;

    let offset = 0;

    const colors = ["#6366f1", "#22c55e", "#f59e0b", "#ef4444"];

    return (
        <Box>
            <Typography variant="subtitle1" mb={1}>
                {title}
            </Typography>

            <Box display="flex" alignItems="center" gap={1}>
                <svg width={size} height={size}>
                    {data.map((d, i) => {
                        const value = selectValue(d);
                        const percent = total === 0 ? 0 : value / total;

                        const circumference = 2 * Math.PI * radius;
                        const dash = percent * circumference;

                        const circle = (
                            <circle
                                key={d.plan}
                                cx={center}
                                cy={center}
                                r={radius}
                                fill="transparent"
                                stroke={colors[i]}
                                strokeWidth={stroke}
                                strokeDasharray={`${dash} ${circumference}`}
                                strokeDashoffset={-offset}
                                strokeLinecap="round"
                            />
                        );

                        offset += dash;
                        return circle;
                    })}
                </svg>

                <Stack spacing={1}>
                    {data.map((d, i) => {
                        const share =
                            total === 0 ? 0 : (selectValue(d) / total) * 100;

                        return (
                            <Stack
                                key={d.plan}
                                direction="row"
                                spacing={1}
                                alignItems="center"
                            >
                                <Box
                                    sx={{
                                        width: 10,
                                        height: 10,
                                        bgcolor: colors[i],
                                        borderRadius: 10,
                                    }}
                                />
                                <Typography variant="body2">
                                    {d.plan} — {share.toFixed(1)}%
                                </Typography>
                            </Stack>
                        );
                    })}
                </Stack>
            </Box>
        </Box>
    );
}

/* ---------------- MAIN DASHBOARD ---------------- */

export default function Dashboard() {
    const [range, setRange] = useState<7 | 30 | 90 | "custom">(30);

    const [startDate, setStartDate] = useState<string>(() => {
        const d = new Date();
        d.setDate(d.getDate() - 30);
        return d.toISOString().slice(0, 10);
    });

    const [endDate, setEndDate] = useState<string>(() =>
        new Date().toISOString().slice(0, 10)
    );

    const [planFilter, setPlanFilter] = useState<Plan | "ALL">("ALL");

    const filtered = useMemo(() => {
        return basePlans.filter((p) =>
            planFilter === "ALL" ? true : p.plan === planFilter
        );
    }, [planFilter]);

    const handleStartDateChange = (value: string) => {
        let start = toDate(value);

        const maxAllowed = endDate
            ? addDays(toDate(endDate), -1)
            : MAX_DATE;

        start = clampDate(start, MIN_DATE, maxAllowed);

        setStartDate(toISO(start));

        const end = toDate(endDate);
        if (end <= start) {
            setEndDate(toISO(addDays(start, 1)));
        }
    };

    const handleEndDateChange = (value: string) => {
        let end = toDate(value);

        const minAllowed = addDays(toDate(startDate), 1);

        end = clampDate(end, minAllowed, MAX_DATE);

        setEndDate(toISO(end));
    };

    const revenue = filtered.reduce((a, b) => a + b.revenue, 0);
    const orderProfit = filtered.reduce((a, b) => a + b.orderProfit, 0);
    const totalProfit = revenue + orderProfit;
    const users = filtered.reduce((a, b) => a + b.users, 0);

    return (
        <Box p={4}>
            <Typography variant="h4" fontWeight={700} mb={2}>
                Аналітика доходів
            </Typography>

            {/* FILTER BAR */}
            <Card sx={{ mb: 3, borderRadius: 3 }}>
                <CardContent>
                    <Stack direction="row" spacing={2} flexWrap="wrap">

                        <FormControl size="small" sx={{ minWidth: 180 }}>
                            <InputLabel>Період дат</InputLabel>
                            <Select
                                value={range}
                                label="Період дат"
                                onChange={(e) =>
                                    setRange(e.target.value as any)
                                }
                            >
                                {[7, 30, 90].map((days) => (
                                    <MenuItem key={days} value={days}>
                                        Останні {days} днів
                                    </MenuItem>
                                ))}
                                <MenuItem value="custom">
                                    Користувацький
                                </MenuItem>
                            </Select>
                        </FormControl>

                        {range === "custom" && (
                            <>
                                <TextField
                                    size="small"
                                    type="date"
                                    value={startDate}
                                    onChange={(e) =>
                                        handleStartDateChange(e.target.value)
                                    }
                                />

                                <TextField
                                    size="small"
                                    type="date"
                                    value={endDate}
                                    onChange={(e) =>
                                        handleEndDateChange(e.target.value)
                                    }
                                />
                            </>
                        )}

                        <FormControl size="small" sx={{ minWidth: 180 }}>
                            <InputLabel>Тариф</InputLabel>
                            <Select
                                value={planFilter}
                                label="Тариф"
                                onChange={(e) =>
                                    setPlanFilter(e.target.value as any)
                                }
                            >
                                <MenuItem value="ALL">Усі</MenuItem>
                                <MenuItem value="Free">Безкоштовний</MenuItem>
                                <MenuItem value="Pro">Професійний</MenuItem>
                                <MenuItem value="Advanced">
                                    Розширений
                                </MenuItem>
                            </Select>
                        </FormControl>

                        <Button variant="contained">
                            Застосувати
                        </Button>
                    </Stack>
                </CardContent>
            </Card>

            {/* KPI */}
            <Grid container spacing={2}>
                <Grid size={{ xs: 12, md: 3 }}>
                    <StatCard title="MRR" value={`$${revenue.toFixed(2)}`} />
                </Grid>
                <Grid size={{ xs: 12, md: 3 }}>
                    <StatCard
                        title="Прибуток з замовлень"
                        value={`$${orderProfit.toFixed(2)}`}
                    />
                </Grid>
                <Grid size={{ xs: 12, md: 3 }}>
                    <StatCard
                        title="Загальний прибуток"
                        value={`$${totalProfit.toFixed(2)}`}
                    />
                </Grid>
                <Grid size={{ xs: 12, md: 3 }}>
                    <StatCard
                        title="Користувачі"
                        value={`${users}`}
                    />
                </Grid>
            </Grid>

            {/* CHARTS */}
            <Grid container spacing={1} mt={2}>
                <Grid size={{ xs: 12, md: 6, lg: 4 }}>
                    <Card>
                        <CardContent>
                            <DonutChart
                                data={filtered}
                                valueKey="profit"
                                title="Загальний прибуток"
                            />
                        </CardContent>
                    </Card>
                </Grid>

                <Grid size={{ xs: 12, md: 6, lg: 4 }}>
                    <Card>
                        <CardContent>
                            <DonutChart
                                data={filtered}
                                valueKey="orderProfit"
                                title="Прибуток з замовлень"
                            />
                        </CardContent>
                    </Card>
                </Grid>

                <Grid size={{ xs: 12, md: 6, lg: 4 }}>
                    <Card>
                        <CardContent>
                            <DonutChart
                                data={filtered}
                                valueKey="mrr"
                                title="MRR"
                            />
                        </CardContent>
                    </Card>
                </Grid>
            </Grid>

            {/* TABLE */}
            <Card sx={{ mt: 3, borderRadius: 3 }}>
                <CardContent>
                    <Typography variant="h6" mb={2}>
                        Розподіл за тарифами
                    </Typography>

                    <Paper variant="outlined">
                        <Table>
                            <TableHead>
                                <TableRow>
                                    <TableCell>Тариф</TableCell>
                                    <TableCell align="right">
                                        Користувачі
                                    </TableCell>
                                    <TableCell align="right">
                                        Дохід від підписки
                                    </TableCell>
                                    <TableCell align="right">
                                        Прибуток із замовлень
                                    </TableCell>
                                </TableRow>
                            </TableHead>

                            <TableBody>
                                {filtered.map((d) => (
                                    <TableRow key={d.plan}>
                                        <TableCell>{d.plan}</TableCell>
                                        <TableCell align="right">
                                            {d.users}
                                        </TableCell>
                                        <TableCell align="right">
                                            ${d.revenue.toFixed(2)}
                                        </TableCell>
                                        <TableCell align="right">
                                            ${d.orderProfit.toFixed(2)}
                                        </TableCell>
                                    </TableRow>
                                ))}
                            </TableBody>
                        </Table>
                    </Paper>
                </CardContent>
            </Card>
        </Box>
    );
}
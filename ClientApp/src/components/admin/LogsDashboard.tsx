import {
    Box,
    Chip,
    Collapse,
    IconButton,
    Paper,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TablePagination,
    TableRow,
    Typography,
    TextField,
    MenuItem,
} from "@mui/material";

import KeyboardArrowDownIcon from "@mui/icons-material/KeyboardArrowDown";
import KeyboardArrowUpIcon from "@mui/icons-material/KeyboardArrowUp";

import { useMemo, useState } from "react";

/* ---------------- TYPES ---------------- */

export enum Severity {
    Error = "Error",
    Critical = "Critical",
}

export interface LogProperties {
    traceId: string;
    machine: string;
    environment: string;
    [key: string]: string
}

export interface LogEntity {
    id: string;
    message: string;
    created: string;
    severity: Severity;
    source: string;
    properties: LogProperties;
}

/* ---------------- helpers ---------------- */

function random<T>(arr: T[]): T {
    return arr[Math.floor(Math.random() * arr.length)];
}

function chance(p: number) {
    return Math.random() < p;
}

/* ---------------- system constants ---------------- */

const queues = [
    "screenshots-regular",
    "screenshots-pro",
    "screenshots-advanced",
];

const blobPath = () =>
    `screenshots/${new Date().getFullYear()}/${crypto.randomUUID()}.png`;

/* ---------------- event generators ---------------- */

function apiGatewayLog(i: number) {
    const stage = random([
        "StripeWebhookReceived",
        "PaymentValidated",
        "DBTransaction",
        "QueuePublished",
    ]);

    const message = {
        StripeWebhookReceived: "Отримано Stripe webhook",
        PaymentValidated: "Платіж успішно підтверджено",
        DBTransaction: "Транзакцію PostgreSQL виконано",
        QueuePublished: "Завдання скриншоту відправлено в RabbitMQ",
    }[stage]!;

    const queueName = stage === "QueuePublished" ? random(queues) : undefined;

    const result: LogEntity = {
        id: crypto.randomUUID(),
        created: new Date(Date.now() - i * 1000 * 60 * 2).toISOString(),
        severity: stage === "PaymentValidated" && chance(0.2) ? Severity.Critical : Severity.Error,
        source: "ApiGateway",
        message,
        properties: {
            traceId: crypto.randomUUID(),
            machine: 'api-gateway',
            environment: "Production",
            service: "ApiGateway",
            stage,
        },
    };

    if (queueName)
        result.properties["queueName"] = queueName;

    return result;
}

function workerLog(i: number): LogEntity {
    const stage = random([
        "QueueConsumed",
        "PlaywrightStarted",
        "ScreenshotCaptured",
        "BlobUpload",
        "JobCompleted",
    ]);

    const result: LogEntity = {
        id: crypto.randomUUID(),
        created: new Date(Date.now() - i * 1000 * 60 * 2 + 500).toISOString(),
        severity:
            stage === "PlaywrightStarted" && chance(0.15)
                ? Severity.Critical
                : Severity.Error,
        source: "ScreenshotWorker",
        message: {
            QueueConsumed: "Повідомлення RabbitMQ оброблено",
            PlaywrightStarted: "Запущено браузер Playwright",
            ScreenshotCaptured: "Скріншот успішно створено",
            BlobUpload: "Завантажено скріншот в Azure Blob Storage",
            JobCompleted: "Завдання скриншоту завершено",
        }[stage]!,
        properties: {
            traceId: crypto.randomUUID(),
            machine: random(["worker-1", "worker-2"]),
            environment: "Production",
            service: "Worker",
            stage,
            queueName: random(queues),
        },
    };

    if (stage === "BlobUpload")
        result.properties["blobName"] = blobPath();

    return result;
}

/* ---------------- main generator ---------------- */

export const mockLogs: LogEntity[] = Array.from({ length: 80 }).flatMap(
    (_, i) => {
        return [
            apiGatewayLog(i),
            workerLog(i),
        ];
    }
);

/* ---------------- SEARCH CONFIG ---------------- */

const SEARCH_FIELDS = [
    "message",
    "traceId",
    "machine",
    "environment",
] as const;

type SearchField = (typeof SEARCH_FIELDS)[number] | "custom";

/* ---------------- LOGIC ---------------- */

function getFieldValue(log: LogEntity, field: string): string | undefined {
    if (field in log) return String((log as any)[field]);
    if (field in log.properties) return String((log.properties as any)[field]);
    return undefined;
}

function matches(log: LogEntity, field: SearchField, key: string, value: string) {
    if (!value) return true;

    const v = value.toLowerCase();

    // CUSTOM MODE
    if (field === "custom") {
        const k = key?.toLowerCase();
        if (!k) return false;

        const raw =
            (log.properties as any)[k] ??
            (log as any)[k];

        return String(raw ?? "").toLowerCase().includes(v);
    }

    // PREDEFINED FIELD MODE
    const raw = getFieldValue(log, field);
    return String(raw ?? "").toLowerCase().includes(v);
}

/* ---------------- UI ---------------- */

function SeverityChip({ severity }: { severity: Severity }) {
    return severity === Severity.Critical ? (
        <Chip label="Критичний" color="error" />
    ) : (
        <Chip label="Помилка" color="warning" />
    );
}

function LogRow({ log }: { log: LogEntity }) {
    const [open, setOpen] = useState(false);

    return (
        <>
            <TableRow hover>
                <TableCell width={60}>
                    <IconButton onClick={() => setOpen(!open)}>
                        {open ? <KeyboardArrowUpIcon /> : <KeyboardArrowDownIcon />}
                    </IconButton>
                </TableCell>

                <TableCell>{log.message}</TableCell>
                <TableCell><SeverityChip severity={log.severity} /></TableCell>
                <TableCell>{log.source}</TableCell>
                <TableCell>{new Date(log.created).toLocaleString()}</TableCell>
            </TableRow>

            <TableRow>
                <TableCell colSpan={5} style={{ padding: 0 }}>
                    <Collapse in={open}>
                        <Box sx={{ p: 2 }}>
                            <Typography variant="subtitle2">Властивості</Typography>
                            <Paper sx={{ p: 2, background: "#111", color: "#0f0" }}>
                                <pre style={{ margin: 0 }}>
                                    {JSON.stringify(log.properties, null, 2)}
                                </pre>
                            </Paper>
                        </Box>
                    </Collapse>
                </TableCell>
            </TableRow>
        </>
    );
}

/* ---------------- DASHBOARD ---------------- */

export default function LogsDashboard() {
    const [page, setPage] = useState(0);
    const [rowsPerPage, setRowsPerPage] = useState(10);

    const [field, setField] = useState<SearchField>("message");
    const [severity, setSeverity] = useState<Severity | "All">("All");
    const [customKey, setCustomKey] = useState("");
    const [value, setValue] = useState("");

    const filtered = useMemo(() => {
        return mockLogs.filter((log) =>
            (log.severity === severity || severity === "All") &&
            matches(log, field, customKey, value)
        );
    }, [field, customKey, value, severity]);

    const paginated = filtered.slice(
        page * rowsPerPage,
        page * rowsPerPage + rowsPerPage
    );

    return (
        <Paper sx={{ width: "100%" }}>
            <Box sx={{ p: 2 }}>
                <Typography variant="h5">Панель логів</Typography>

                {/* SEARCH BAR */}
                <Box sx={{ display: "flex", gap: 2, mt: 2 }}>
                    <TextField
                        fullWidth
                        label="Значення пошуку"
                        value={value}
                        onChange={(e) => {
                            setValue(e.target.value);
                            setPage(0);
                        }}
                        size="small"
                    />
                </Box>

                <Box sx={{ display: "flex", gap: 2, mt: 2 }}>
                    <TextField
                        sx={{ minWidth: 180 }}
                        select
                        label="Поле"
                        value={field}
                        onChange={(e) => setField(e.target.value as SearchField)}
                        size="small"
                    >
                        {SEARCH_FIELDS.map((f) => (
                            <MenuItem key={f} value={f}>
                                {f}
                            </MenuItem>
                        ))}
                        <MenuItem value="custom">власне</MenuItem>
                    </TextField>

                    {field === "custom" && (
                        <TextField
                            sx={{ minWidth: 180 }}
                            label="Назва власного поля"
                            value={customKey}
                            onChange={(e) => setCustomKey(e.target.value)}
                            size="small"
                        />
                    )}
                </Box>

                <Box sx={{ display: "flex", gap: 2, mt: 2 }}>
                    <TextField
                        sx={{ minWidth: 180 }}
                        select
                        label="Серйозність"
                        value={severity}
                        onChange={(e) =>
                            setSeverity(e.target.value === "All" ? "All" : e.target.value as Severity)
                        }
                        size="small"
                    >
                        <MenuItem value="All">Усі</MenuItem>
                        {[Severity.Critical, Severity.Error].map((f) => (
                            <MenuItem key={f} value={f}>
                                {f === Severity.Critical ? "Критичний" : "Помилка"}
                            </MenuItem>
                        ))}
                    </TextField>
                </Box>
            </Box>

            <TableContainer>
                <Table>
                    <TableHead>
                        <TableRow>
                            <TableCell />
                            <TableCell>Повідомлення</TableCell>
                            <TableCell>Серйозність</TableCell>
                            <TableCell>Джерело</TableCell>
                            <TableCell>Створено</TableCell>
                        </TableRow>
                    </TableHead>

                    <TableBody>
                        {paginated.map((log) => (
                            <LogRow key={log.id} log={log} />
                        ))}
                    </TableBody>
                </Table>
            </TableContainer>

            <TablePagination
                component="div"
                count={filtered.length}
                page={page}
                onPageChange={(_, p) => setPage(p)}
                rowsPerPage={rowsPerPage}
                onRowsPerPageChange={(e) => {
                    setRowsPerPage(parseInt(e.target.value, 10));
                    setPage(0);
                }}
            />
        </Paper>
    );
}
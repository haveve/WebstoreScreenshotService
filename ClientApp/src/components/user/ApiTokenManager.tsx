import React, { useState } from "react";
import {
    Box,
    Button,
    Chip,
    Container,
    Dialog,
    DialogActions,
    DialogContent,
    DialogTitle,
    FormControlLabel,
    FormGroup,
    Checkbox,
    Stack,
    TextField,
    Typography,
} from "@mui/material";

/* ---------------- SCOPES ---------------- */

const SCOPES = [
    { key: "user.screenshots.fetch", label: "Отримання скріншотів" },
    { key: "user.screenshots.make", label: "Створення скріншотів" },
];

/* ---------------- HELPERS ---------------- */

const now = () => new Date().toISOString();

const base64Url = (obj: any) =>
    btoa(JSON.stringify(obj))
        .replace(/\+/g, "-")
        .replace(/\//g, "_")
        .replace(/=+$/, "");

export const generateToken = () => {
    const header = {
        alg: "HS256",
        typ: "JWT",
    };

    const payload = {
        sub: crypto.randomUUID(),
        iat: Math.floor(Date.now() / 1000),
        exp: Math.floor(Date.now() / 1000) + 60 * 60,
        scope: "api",
    };

    const signature = Array.from({ length: 32 }, () =>
        Math.floor(Math.random() * 36).toString(36)
    ).join("");

    return "api_v1_" + [
        base64Url(header),
        base64Url(payload),
        signature,
    ].join(".");
};

const getMockLocation = () => ({
    countryCode: "UA",
    country: "Україна",
    city: "Київ",
});

/* ---------------- VALIDATION ---------------- */

const isValidIPv4 = (ip: string) =>
    /^(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}$/.test(ip);

const isValidCIDR = (ip: string) =>
    /^(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}\/([0-9]|[12]\d|3[0-2])$/.test(ip);

const isValidIPv6 = (ip: string) =>
    /^[0-9a-fA-F:]+$/.test(ip) && ip.includes(":");

const validateIp = (ip: string) =>
    isValidIPv4(ip) || isValidCIDR(ip) || isValidIPv6(ip);

/* ---------------- TYPES ---------------- */

interface TokenLocation {
    countryCode: string;
    country: string;
    city?: string;
}

interface ApiTokenMetadata {
    issued: string;
    issuedLocation?: TokenLocation;

    lastUsed?: string;
    lastUsedLocation?: TokenLocation;

    revoked?: string;
    revokeLocation?: TokenLocation;
}

interface ApiToken {
    id: string;
    name: string;
    scopes: string[];
    allowedIps: string[];
    revoked: boolean;
    revokedReason?: string;
    createdAt: string;
    tokenMetadata?: ApiTokenMetadata;
}

/* ---------------- MOCK ---------------- */

const mockTokens: ApiToken[] = [
    {
        id: "1",
        name: "Сервіс скріншотів — продакшн",
        scopes: ["user.screenshots.fetch", "user.screenshots.make"],
        allowedIps: ["192.168.1.10", "10.0.0.0/24"],
        revoked: false,
        createdAt: "2026-05-20T10:12:00Z",
        tokenMetadata: {
            issued: "2026-05-20T10:12:00Z",
            issuedLocation: {
                countryCode: "UA",
                country: "Україна",
                city: "Київ",
            },
            lastUsed: "2026-05-22T09:10:00Z",
            lastUsedLocation: {
                countryCode: "PL",
                country: "Польща",
                city: "Варшава",
            },
        },
    },
];

/* ---------------- COMPONENT ---------------- */

export default function ApiTokenManager() {
    const [tokens, setTokens] = useState<ApiToken[]>(mockTokens);

    const [openCreate, setOpenCreate] = useState(false);
    const [openRevoke, setOpenRevoke] = useState(false);

    const [selected, setSelected] = useState<ApiToken | null>(null);
    const [createdToken, setCreatedToken] = useState<string | null>(null);

    const [name, setName] = useState("");
    const [scopes, setScopes] = useState<string[]>([]);
    const [ipInput, setIpInput] = useState("");
    const [ipList, setIpList] = useState<string[]>([]);
    const [ipError, setIpError] = useState<string | null>(null);

    const [revokeReason, setRevokeReason] = useState("");

    /* ---------------- CREATE ---------------- */

    const createToken = () => {
        const raw = generateToken();
        const location = getMockLocation();

        const newToken: ApiToken = {
            id: String(Date.now()),
            name,
            scopes,
            allowedIps: ipList,
            revoked: false,
            createdAt: now(),
            tokenMetadata: {
                issued: now(),
                issuedLocation: location,
            },
        };

        setTokens((prev) => [newToken, ...prev]);
        setCreatedToken(raw);

        setOpenCreate(false);
        setName("");
        setScopes([]);
        setIpList([]);
    };

    /* ---------------- REVOKE ---------------- */

    const revokeToken = () => {
        if (!selected) return;

        const location = getMockLocation();

        setTokens((prev) =>
            prev.map((t) =>
                t.id === selected.id
                    ? {
                        ...t,
                        revoked: true,
                        revokedReason: revokeReason,
                        tokenMetadata: {
                            ...t.tokenMetadata!,
                            revoked: now(),
                            revokeLocation: location,
                        },
                    }
                    : t
            )
        );

        setSelected(null);
        setRevokeReason("");
        setOpenRevoke(false);
    };

    /* ---------------- IP ---------------- */

    const addIp = () => {
        const value = ipInput.trim();
        if (!value) return;

        if (!validateIp(value)) {
            setIpError("Невірний формат IPv4 / IPv6 / CIDR");
            return;
        }

        setIpList((prev) => [...prev, value]);
        setIpInput("");
        setIpError(null);
    };

    /* ---------------- UI ---------------- */

    return (
        <Container maxWidth="md" sx={{ mt: 4 }}>
            {/* HEADER */}
            <Stack direction="row" justifyContent="space-between" mb={2}>
                <Typography variant="h5" fontWeight={700}>
                    Керування API токенами
                </Typography>

                <Button variant="contained" onClick={() => setOpenCreate(true)}>
                    Створити токен
                </Button>
            </Stack>

            {/* LIST */}
            <Stack spacing={2}>
                {tokens.map((t) => (
                    <Box key={t.id} sx={{ border: "1px solid #eee", borderRadius: 2, p: 2 }}>
                        <Stack direction="row" justifyContent="space-between">
                            <Box>
                                <Typography fontWeight={700}>{t.name}</Typography>

                                <Stack direction="row" spacing={1} mt={1}>
                                    <Chip
                                        label={t.revoked ? "ВІДКЛЮЧЕНО" : "АКТИВНИЙ"}
                                        sx={{
                                            backgroundColor: t.revoked ? "#d32f2f" : "#2e7d32",
                                            color: "#fff",
                                            fontWeight: 700,
                                        }}
                                        size="small"
                                    />
                                </Stack>
                            </Box>

                            <Button
                                size="small"
                                color="error"
                                disabled={t.revoked}
                                onClick={() => {
                                    setSelected(t);
                                    setOpenRevoke(true);
                                }}
                            >
                                Відкликати
                            </Button>
                        </Stack>

                        {/* LOCATION */}
                        <Box mt={2}>
                            <Typography variant="caption" fontWeight={700} color="text.secondary">
                                Метадані місцезнаходження
                            </Typography>

                            <Stack spacing={0.6} mt={1}>
                                <Typography variant="caption">
                                    🌍 <b>Видано:</b>{" "}
                                    {t.tokenMetadata?.issuedLocation
                                        ? `${t.tokenMetadata.issued} · ${t.tokenMetadata.issuedLocation.country} · ${t.tokenMetadata.issuedLocation.city ?? ""}`
                                        : "—"}
                                </Typography>

                                <Typography variant="caption">
                                    🕒 <b>Останнє використання:</b>{" "}
                                    {t.tokenMetadata?.lastUsedLocation
                                        ? `${t.tokenMetadata.lastUsed} · ${t.tokenMetadata.lastUsedLocation.country} · ${t.tokenMetadata.lastUsedLocation.city ?? ""}`
                                        : "Ніколи"}
                                </Typography>

                                {t.revoked && (
                                    <Typography variant="caption" color="error.main">
                                        🚫 <b>Відкликано (
                                            {t.tokenMetadata?.revokeLocation
                                                ? `${t.tokenMetadata?.revoked} · ${t.tokenMetadata?.revokeLocation.country} · ${t.tokenMetadata.revokeLocation.city ?? ""}`
                                                : ""}):</b>{" "}
                                        {t.revokedReason || "без причини"}
                                    </Typography>
                                )}
                            </Stack>
                        </Box>

                        <Box mt={1}>
                            <Typography variant="caption" fontWeight={600}>
                                Дозволи ({t.scopes.length})
                            </Typography>
                            <Stack direction="row" spacing={1} flexWrap="wrap" mt={1}>
                                {t.scopes.map((scope) => (
                                    <Chip key={scope} label={scope} size="small" />
                                ))}
                            </Stack>
                        </Box>

                        {/* IP */}
                        <Box mt={1}>
                            <Typography variant="caption" fontWeight={600}>
                                Дозволені IP ({t.allowedIps.length})
                            </Typography>

                            <Stack direction="row" spacing={1} flexWrap="wrap">
                                {t.allowedIps.map((ip) => (
                                    <Chip key={ip} label={ip} size="small" />
                                ))}
                            </Stack>
                        </Box>
                    </Box>
                ))}
            </Stack>

            {/* CREATED TOKEN */}
            <Dialog open={!!createdToken} onClose={() => setCreatedToken(null)} fullWidth>
                <DialogTitle>Токен створено</DialogTitle>
                <DialogContent>
                    <Typography color="warning.main">
                        Збережіть токен — він показується лише один раз
                    </Typography>

                    <Box
                        sx={{
                            p: 2,
                            fontFamily: "monospace",
                            background: "#f5f5f5",
                            wordBreak: "break-all",
                            overflowWrap: "anywhere",
                            whiteSpace: "pre-wrap",
                        }}
                    >
                        {createdToken}
                    </Box>
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setCreatedToken(null)}>Закрити</Button>
                </DialogActions>
            </Dialog>

            {/* CREATE */}
            <Dialog open={openCreate} onClose={() => setOpenCreate(false)} fullWidth>
                <DialogTitle>Створення токена</DialogTitle>
                <DialogContent>
                    <Stack spacing={2} py={1}>
                        <TextField
                            label="Назва"
                            value={name}
                            onChange={(e) => setName(e.target.value)}
                            fullWidth
                        />

                        <Typography fontWeight={600}>Сфери доступу</Typography>

                        <FormGroup>
                            {SCOPES.map((s) => (
                                <FormControlLabel
                                    key={s.key}
                                    control={
                                        <Checkbox
                                            checked={scopes.includes(s.key)}
                                            onChange={(e) =>
                                                setScopes((p) =>
                                                    e.target.checked
                                                        ? [...p, s.key]
                                                        : p.filter((x) => x !== s.key)
                                                )
                                            }
                                        />
                                    }
                                    label={s.label}
                                />
                            ))}
                        </FormGroup>

                        <TextField
                            label="IP / CIDR"
                            value={ipInput}
                            error={!!ipError}
                            helperText={ipError}
                            onChange={(e) => setIpInput(e.target.value)}
                            onKeyDown={(e) => e.key === "Enter" && addIp()}
                            fullWidth
                        />

                        <Typography variant="body2" color="text.secondary">
                            Дозволені формати: IP, CIDR, IPv6
                        </Typography>

                        <Button onClick={addIp} variant="outlined">
                            Додати IP
                        </Button>

                        <Stack direction="row" spacing={1} flexWrap="wrap">
                            {ipList.map((ip) => (
                                <Chip key={ip} label={ip} />
                            ))}
                        </Stack>
                    </Stack>
                </DialogContent>

                <DialogActions>
                    <Button onClick={() => setOpenCreate(false)}>Скасувати</Button>
                    <Button variant="contained" onClick={createToken}>
                        Створити
                    </Button>
                </DialogActions>
            </Dialog>

            {/* REVOKE */}
            <Dialog open={openRevoke && !!selected} onClose={() => setOpenRevoke(false)} fullWidth>
                <DialogTitle>Відкликання токена</DialogTitle>
                <DialogContent>
                    {selected && (
                        <TextField
                            label="Причина"
                            value={revokeReason}
                            onChange={(e) => setRevokeReason(e.target.value)}
                            fullWidth
                        />
                    )}
                </DialogContent>

                <DialogActions>
                    <Button onClick={() => setOpenRevoke(false)}>Скасувати</Button>
                    <Button color="error" variant="contained" onClick={revokeToken}>
                        Відкликати
                    </Button>
                </DialogActions>
            </Dialog>
        </Container>
    );
}
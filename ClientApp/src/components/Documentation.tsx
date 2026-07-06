import React, { useState } from "react";
import {
    Box,
    Container,
    Typography,
    Accordion,
    AccordionSummary,
    AccordionDetails,
    Chip,
    Stack,
    Paper,
    Divider,
} from "@mui/material";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";

/* ---------------- MODELS ---------------- */


const models: any = {
    InputScreenshotModel: {
        description: "Main request model for creating a screenshot.",
        fields: [
            {
                name: "url",
                type: "string",
                required: true,
                description: "Target webpage URL to capture screenshot from.",
                validation: {
                    maxLength: 2048,
                    format: "Valid absolute URL (http/https)",
                    rules: ["Must be safe URL", "No javascript: or data: schemes"],
                },
            },
            {
                name: "mode",
                type: '"High" | "Medium" | "Low"',
                required: true,
                description: "Quality mode controlling rendering fidelity and performance.",
                validation: {
                    allowedValues: ["High", "Medium", "Low"],
                },
            },
            {
                name: "screenshotType",
                type: '"Png" | "Jpeg"',
                required: true,
                description: "Output image format.",
                validation: {
                    allowedValues: ["Png", "Jpeg"],
                },
            },

            {
                name: "clip",
                type: "object",
                required: false,
                description: "Defines clipping region of screenshot.",
                validation: {
                    exclusiveWith: "element",
                },
                children: [
                    {
                        name: "width",
                        type: "number",
                        required: true,
                        description: "Width of clipping area in pixels.",
                        validation: {
                            range: { min: 1, max: 5000 },
                        },
                    },
                    {
                        name: "height",
                        type: "number",
                        required: false,
                        description: "Optional height. If omitted, full page height is used.",
                        validation: {
                            range: { min: 1, max: 7000 },
                        },
                    },
                ],
            },

            {
                name: "element",
                type: "object",
                required: false,
                description: "Capture only a specific DOM element.",
                validation: {
                    exclusiveWith: "clip",
                },
                children: [
                    {
                        name: "selector",
                        type: "string",
                        required: true,
                        description: "CSS selector of target element.",
                        validation: {
                            maxLength: 200,
                            safeCss: true,
                        },
                    },
                    {
                        name: "clip",
                        type: "object",
                        required: true,
                        description: "Element clipping configuration.",
                        children: [
                            {
                                name: "width",
                                type: "number",
                                required: true,
                                validation: { range: { min: 1, max: 5000 } },
                            },
                            {
                                name: "height",
                                type: "number",
                                required: true,
                                validation: { range: { min: 1, max: 7000 } },
                            },
                        ],
                    },
                ],
            },

            {
                name: "advancedConfiguration",
                type: "object",
                required: false,
                description: "Advanced browser rendering configuration.",
                children: [
                    {
                        name: "locale",
                        type: "string",
                        required: true,
                        description: "Browser locale (e.g. en-US).",
                        validation: {
                            pattern: "^[a-z]{2}-[A-Z]{2}$",
                        },
                    },
                    {
                        name: "timezoneId",
                        type: "string",
                        required: true,
                        description: "Timezone (IANA format).",
                        validation: {
                            pattern: "^[A-Za-z]+/[A-Za-z_]+$",
                        },
                    },
                    {
                        name: "colorScheme",
                        type: '"Light" | "Dark" | "NoPreference"',
                        required: true,
                        description: "Rendering theme mode.",
                        validation: {
                            allowedValues: ["Light", "Dark", "NoPreference"],
                        },
                    },
                    {
                        name: "waitForSelector",
                        type: "string",
                        required: false,
                        description: "Wait for element before screenshot.",
                        validation: {
                            maxLength: 200,
                            safeCss: true,
                        },
                    },
                    {
                        name: "blockResources",
                        type: "number",
                        required: true,
                        description: "Resource blocking bitmask.",
                        validation: {
                            range: { min: 0, max: 31 },
                        },
                    },
                    {
                        name: "headers",
                        type: "HeaderModel[]",
                        required: false,
                        description: "Custom HTTP headers.",
                        validation: {
                            maxItems: 20,
                        },
                    },
                    {
                        name: "cookies",
                        type: "CookieModel[]",
                        required: false,
                        description: "Injected cookies.",
                        validation: {
                            maxItems: 20,
                        },
                    },
                ],
            },
        ],
    },

    CookieModel: {
        description:
            "Represents a browser cookie injected into the screenshot session.",
        fields: [
            {
                name: "name",
                type: "string",
                required: true,
                description: "Cookie name.",
                validation: {
                    pattern: "^[a-zA-Z0-9_-]+$",
                    maxLength: 100,
                },
            },
            {
                name: "value",
                type: "string",
                required: true,
                description: "Cookie value.",
                validation: {
                    maxLength: 1000,
                    noControlChars: true,
                },
            },
            {
                name: "domain",
                type: "string",
                required: true,
                description: "Cookie domain.",
                validation: {
                    pattern: "^\\..+",
                    maxLength: 200,
                },
            },
            {
                name: "path",
                type: "string",
                required: false,
                description: "Cookie path.",
                validation: {
                    maxLength: 100,
                },
            },
            {
                name: "sameSite",
                type: '"Strict" | "Lax" | "None"',
                required: false,
                description: "CSRF protection level.",
                validation: {
                    allowedValues: ["Strict", "Lax", "None"],
                },
            },
        ],
    },

    HeaderModel: {
        description:
            "Custom HTTP header for request customization.",
        fields: [
            {
                name: "name",
                type: "string",
                required: true,
                description: "Header name.",
                validation: {
                    maxLength: 100,
                    pattern: "^[a-zA-Z0-9-]+$",
                },
            },
            {
                name: "value",
                type: "string",
                required: true,
                description: "Header value.",
                validation: {
                    maxLength: 1000,
                },
            },
        ],
    },

    ScreenshotModel: {
        description: "Screenshot response model.",
        fields: [
            { name: "id", type: "string", required: true },
            { name: "websiteUrl", type: "string", required: true },
            { name: "url", type: "string", required: true },
            { name: "createdAt", type: "string", required: true },
            { name: "state", type: "string", required: true },
            {
                name: "type",
                type: '"Png" | "Jpeg"',
                required: true,
                validation: {
                    allowedValues: ["Png", "Jpeg"],
                },
            },
        ],
    },

    Paging: {
        description: "Pagination request model.",
        fields: [
            {
                name: "page",
                type: "number",
                required: true,
                validation: { range: { min: 1, max: 10000 } },
            },
            {
                name: "pageSize",
                type: "number",
                required: true,
                validation: { range: { min: 1, max: 200 } },
            },
        ],
    },

    PaginationResult: {
        description: "Generic paginated response.",
        fields: [
            { name: "totalCount", type: "number", required: true },
            { name: "items", type: "ScreenshotModel[]", required: true },
        ],
    },

    ErrorResponse: {
        description: "Standard API error response.",
        fields: [
            { name: "message", type: "string", required: true },
        ],
    },
};

/* ---------------- ENDPOINTS ---------------- */

const endpoints = [
    {
        name: "Make Screenshot",
        method: "POST",
        route: "screenshots/makeScreenshot",
        auth: "User + Policy: user.screenshots.make",
        description:
            "Captures a screenshot of a webpage using advanced rendering options.",
        requestModel: "InputScreenshotModel",
        example: {
            url: "https://example.com",
            mode: "High",
            screenshotType: "Png",
        },
        responses: [
            {
                code: 200,
                desc: "Screenshot successfully created",
            },
            {
                code: 400,
                desc: "Invalid input or user limit exceeded",
            },
            {
                code: 401,
                desc: "Unauthorized",
            },
        ],
    },

    {
        name: "Calculate Screenshot Cost",
        method: "POST",
        route: "screenshots/calculateScreenshotCost",
        auth: "User + Policy: user.screenshots.make",
        description: "Returns cost estimation for screenshot request.",
        requestModel: "InputScreenshotModel",
        example: {
            url: "https://example.com",
            mode: "Medium",
        },
        responses: [
            { code: 200, desc: "Cost calculated successfully" },
            { code: 400, desc: "Invalid request" },
            {
                code: 401,
                desc: "Unauthorized",
            },
        ],
    },

    {
        name: "Get Screenshots",
        method: "GET",
        route: "screenshots/getScreenshots",
        auth: "User + Policy: user.screenshots.fetch",
        description: "Returns paginated screenshots list.",
        requestModel: "Paging",
        example: {
            page: 1,
            pageSize: 10,
        },
        responses: [
            { code: 200, desc: "Paged list of screenshots" },
            { code: 400, desc: "Invalid request" },
            { code: 401, desc: "Unauthorized" },
        ],
    },

    {
        name: "Get Screenshot By Id",
        method: "GET",
        route: "screenshots/getScreenshot",
        auth: "User + Policy: user.screenshots.fetch",
        description: "Returns single screenshot by id.",
        requestModel: "string (query param: id)",
        example: {
            id: "abc123",
        },
        responses: [
            { code: 200, desc: "Screenshot found" },
            { code: 400, desc: "Not found or invalid id" },
            {
                code: 401,
                desc: "Unauthorized",
            },
        ],
    },
];

/* ---------------- UI ---------------- */

export default function ApiDocumentationPage() {
    const [expanded, setExpanded] = useState<string | false>(false);

    return (
        <Container maxWidth="md" sx={{ py: 4 }}>
            <Typography variant="h4" fontWeight={800} gutterBottom>
                API Documentation
            </Typography>

            <Typography variant="body2" color="text.secondary" mb={3}>
                Screenshot Service API
            </Typography>

            {endpoints.map((ep, idx) => (
                <Accordion
                    key={idx}
                    expanded={expanded === ep.name}
                    onChange={() =>
                        setExpanded(expanded === ep.name ? false : ep.name)
                    }
                >
                    <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                        <Stack direction="row" spacing={2} alignItems="center">
                            <Chip
                                label={ep.method}
                                color={ep.method === "POST" ? "primary" : "secondary"}
                                size="small"
                            />
                            <Typography fontWeight={700}>{ep.name}</Typography>
                        </Stack>
                    </AccordionSummary>

                    <AccordionDetails>
                        <Stack spacing={2}>
                            {/* ROUTE */}
                            <Paper sx={{ p: 2, bgcolor: "#fafafa" }}>
                                <Typography fontWeight={600}>Route</Typography>
                                <Typography fontFamily="monospace">
                                    {ep.route}
                                </Typography>
                            </Paper>

                            {/* AUTH */}
                            <Box>
                                <Typography fontWeight={600}>Authorization</Typography>
                                <Typography variant="body2" color="secondary">
                                    {ep.auth}
                                </Typography>
                            </Box>

                            {/* DESCRIPTION */}
                            <Box>
                                <Typography fontWeight={600}>Description</Typography>
                                <Typography variant="body2">
                                    {ep.description}
                                </Typography>
                            </Box>

                            <Divider />

                            {/* REQUEST */}
                            <Box>
                                <Typography fontWeight={700} mb={1}>
                                    Request
                                </Typography>

                                <Typography variant="body2">
                                    Type:{" "}
                                    <b>{ep.requestModel}</b>
                                </Typography>

                                <Box mt={1} p={2}>
                                    <pre style={{ margin: 0 }}>
                                        {JSON.stringify(ep.example, null, 2)}
                                    </pre>
                                </Box>
                            </Box>

                            {/* RESPONSES */}
                            <Box>
                                <Typography fontWeight={700} mb={1}>
                                    Response Codes
                                </Typography>

                                <Stack spacing={1}>
                                    {ep.responses.map((r, i) => (
                                        <Stack
                                            key={i}
                                            direction="row"
                                            spacing={2}
                                            alignItems="center"
                                        >
                                            <Chip
                                                size="small"
                                                label={r.code}
                                                color={
                                                    r.code === 200
                                                        ? "success"
                                                        : r.code === 400
                                                            ? "warning"
                                                            : "error"
                                                }
                                            />
                                            <Typography variant="body2">
                                                {r.desc}
                                            </Typography>
                                        </Stack>
                                    ))}
                                </Stack>
                            </Box>
                        </Stack>
                    </AccordionDetails>
                </Accordion>
            ))}

            {/* ---------------- MODELS SECTION ---------------- */}

            <Box mt={5}>
                <Typography variant="h5" fontWeight={800} gutterBottom>
                    Models
                </Typography>

                {Object.entries(models).map(([name, model]: any) => (
                    <Accordion key={name}>
                        <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                            <Typography fontWeight={700}>{name}</Typography>
                        </AccordionSummary>

                        <AccordionDetails>
                            <Typography variant="body2" mb={2}>
                                {model.description}
                            </Typography>

                            {model.fields.map((f: any, i: number) => (
                                <Box
                                    key={i}
                                    sx={{
                                        mb: 2,
                                        p: 2,
                                        border: "1px solid #eee",
                                        borderRadius: 1,
                                    }}
                                >
                                    <Stack direction="row" spacing={1}>
                                        <Typography fontWeight={700}>
                                            {f.name}
                                        </Typography>

                                        <Chip size="small" label={f.type} />

                                        {f.required && (
                                            <Chip
                                                size="small"
                                                color="error"
                                                label="required"
                                            />
                                        )}
                                    </Stack>

                                    <Typography variant="body2" mt={1}>
                                        {f.description}
                                    </Typography>

                                    {/* ✅ ONLY ADDITION */}
                                    {!!f.validation && (
                                        <Typography
                                            variant="caption"
                                            sx={{
                                                display: "block",
                                                mt: 0.5,
                                                color: "warning.main",
                                                fontWeight: 600,
                                            }}
                                        >
                                            {formatValidation(f.validation)}
                                        </Typography>
                                    )}

                                    {f.children && (
                                        <Box mt={2}>
                                            <Typography variant="caption">
                                                Nested
                                            </Typography>

                                            {f.children.map(
                                                (c: any, j: number) => (
                                                    <Box key={j} ml={2} mt={1}>
                                                        <Typography fontWeight={600}>
                                                            {c.name}
                                                        </Typography>

                                                        <Typography variant="body2">
                                                            {c.description}
                                                        </Typography>

                                                        {!!c.validation && (
                                                            <Typography
                                                                variant="caption"
                                                                color="warning.main"
                                                            >
                                                                Validation:{" "}
                                                                {formatValidation(c.validation)}
                                                            </Typography>
                                                        )}
                                                    </Box>
                                                )
                                            )}
                                        </Box>
                                    )}
                                </Box>
                            ))}
                        </AccordionDetails>
                    </Accordion>
                ))}
            </Box>
        </Container>
    );
}

const formatValidation = (v: any) => {
    if (!v) return null;

    const items: string[] = [];

    if (v.allowedValues) {
        items.push(`allowed: ${v.allowedValues.join(", ")}`);
    }

    if (v.maxLength !== undefined) {
        items.push(`max length: ${v.maxLength}`);
    }

    if (v.minLength !== undefined) {
        items.push(`min length: ${v.minLength}`);
    }

    if (v.range) {
        items.push(`range: ${v.range.min} - ${v.range.max}`);
    }

    if (v.pattern) {
        items.push(`pattern: ${v.pattern}`);
    }

    if (v.exclusiveWith) {
        items.push(`exclusive with: ${v.exclusiveWith}`);
    }

    if (v.maxItems !== undefined) {
        items.push(`max items: ${v.maxItems}`);
    }

    if (v.format) {
        items.push(`format: ${v.format}`);
    }

    if (v.rules) {
        items.push(`rules: ${v.rules.join(", ")}`);
    }

    if (v.safeCss) {
        items.push(`safe CSS selector`);
    }

    if (v.safeJs) {
        items.push(`safe JS string`);
    }

    if (v.noControlChars) {
        items.push(`no control characters`);
    }

    return (
        <Box sx={{ mt: 0.5 }}>
            <Typography variant="caption" color="warning.main" fontWeight={850}>
                Validation
            </Typography>

            <ul style={{ margin: "4px 0 0 16px", padding: 0 }}>
                {items.map((item, i) => (
                    <li key={i}>
                        <Typography variant="caption" color="warning">
                            {item}
                        </Typography>
                    </li>
                ))}
            </ul>
        </Box>
    );
};
import { useMemo } from "react";
import {
    Card,
    CardContent,
    Typography,
    Stack,
    Divider,
    Chip,
    LinearProgress,
    Box,
} from "@mui/material";
import { ScreenshotOptionsModel, ScreenshotQualityMode } from "../../behavior/types";

export function calculateScreenshotPrice(options: ScreenshotOptionsModel) {
    const pixels = calculatePixels(options);

    const sizeMultiplier = pixels / (1920 * 1080);

    let total = 1 * sizeMultiplier;

    if (isFullPage(options)) total *= 1.4;
    if (options.element) total *= 1.5;

    total *= getLoadingMultiplier(options);
    total += getAdvancedCost(options.advancedConfiguration);

    if (options.highlightWord) total += 0.5;

    if (options.modalModel?.hideSelectors) {
        total += options.modalModel.hideSelectors.length * 0.2;
    }

    total = Math.max(total, 1);
    total = Math.min(total, 20);

    return {
        points: Math.ceil(total),
        raw: total,
        pixels,
        sizeMultiplier,
    };
}

function calculatePixels(options: any) {
    const MAX_W = 5000;
    const MAX_H = 7000;

    let w = options.clip?.width ?? 1920;
    let h = options.clip?.height ?? 1080;

    w = Math.min(w, MAX_W);
    h = Math.min(h, MAX_H);

    return w * h;
}

function isFullPage(options: any) {
    return options.clip && options.clip.height == null;
}

function getLoadingMultiplier(v: ScreenshotOptionsModel) {
    switch (v.mode) {
        case ScreenshotQualityMode.Low: return 1;
        case ScreenshotQualityMode.Medium: return v.element ? 1.3 : 1;
        case ScreenshotQualityMode.High: return v.element ? 1.3 : 1.8;
        default: return 1;
    }
}

function getAdvancedCost(adv?: any) {
    if (!adv) return 0;

    let cost = 0;

    if (adv.locale !== "en-US") cost += 0.2;
    if (adv.timezoneId !== "UTC") cost += 0.2;

    if (adv.waitForSelector) cost += 2;

    if (adv.headers?.length) cost += adv.headers.length * 0.15;
    if (adv.cookies?.length) cost += adv.cookies.length * 0.2;

    const b = adv.blockResources ?? 0;

    if (b & 1) cost -= 0.2;
    if (b & 2) cost -= 0.1;
    if (b & 4) cost -= 0.3;
    if (b & 8) cost -= 0.4;
    if (b & 16) cost -= 0.1;

    return Math.max(cost, 0);
}

export function useScreenshotPricing(options: ScreenshotOptionsModel) {
    return useMemo(() => {
        const result = calculateScreenshotPrice(options);

        return {
            ...result,
            usd: (result.points * 0.01).toFixed(3),
        };
    }, [options]);
}

export function CostCalculation({ options }: { options: ScreenshotOptionsModel }) {
    const pricing = useScreenshotPricing(options);

    const progress = Math.min((pricing.points / 20) * 100, 100);

    return (
        <Card sx={{ borderRadius: 3, p: 1, maxWidth: 420 }}>
            <CardContent>

                {/* HEADER */}
                <Typography variant="h6" fontWeight={600}>
                    Screenshot Cost
                </Typography>

                <Typography variant="body2" color="text.secondary">
                    Live estimation based on configuration
                </Typography>

                <Divider sx={{ my: 2 }} />

                {/* PRICE */}
                <Box textAlign="center">
                    <Typography variant="h2" fontWeight={700}>
                        {pricing.points}
                    </Typography>

                    <Typography variant="body2" color="text.secondary">
                        points (~${pricing.usd})
                    </Typography>

                    <LinearProgress
                        variant="determinate"
                        value={progress}
                        sx={{ mt: 2, height: 8, borderRadius: 5 }}
                    />
                </Box>

                <Divider sx={{ my: 2 }} />

                {/* DETAILS */}
                <Stack spacing={1}>

                    <Chip
                        label={`Pixels: ${pricing.pixels.toLocaleString()}`}
                        size="small"
                    />

                    <Chip
                        label={`Size multiplier: x${pricing.sizeMultiplier.toFixed(2)}`}
                        size="small"
                    />

                    <Chip
                        label={`Raw cost: ${pricing.raw.toFixed(2)}`}
                        size="small"
                    />

                </Stack>

            </CardContent>
        </Card>
    );
}
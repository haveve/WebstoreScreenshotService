import { useMemo, useState } from "react";
import {
    Card,
    CardContent,
    Typography,
    Stack,
    Divider,
    Chip,
    LinearProgress,
    Box,
    Button,
} from "@mui/material";
import { ScreenshotOptionsModel, ScreenshotQualityMode } from "../../behavior/types";
import { useTranslation } from "react-i18next";

const MinPoints = 1;
const MaxPoints = 20;

type CalculationResult = {
    points: number;
    pixels: number
}

export function calculateScreenshotPrice(options: ScreenshotOptionsModel): CalculationResult {
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

    total = Math.max(total, MinPoints);
    total = Math.min(total, MaxPoints);

    return {
        points: Math.ceil(total),
        pixels,
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

export function CostCalculation({ options }: { options: ScreenshotOptionsModel }) {
    const { t } = useTranslation();
    const [pricing, setPricing] = useState(calculateScreenshotPrice(options));
    const progress = Math.min((pricing.points / MaxPoints) * 100, 100);

    return (
        <Card sx={{ borderRadius: 3, p: 1, maxWidth: 420 }}>
            <CardContent>

                {/* HEADER */}
                <Typography variant="h6" fontWeight={600}>
                    Вартість скріншота
                </Typography>

                <Typography variant="body2" color="text.secondary">
                    Оцінка в реальному часі на основі конфігурації
                </Typography>

                <Divider sx={{ my: 2 }} />

                {/* PRICE */}
                <Box textAlign="center">
                    <Typography variant="h2" fontWeight={700}>
                        {pricing.points}
                    </Typography>

                    <Typography variant="subtitle1" fontWeight={700}>
                        Бали
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
                        label={`Пікселі: ${pricing.pixels.toLocaleString()}`}
                        size="small"
                    />
                </Stack>

                <Box textAlign="center">
                    <Button
                        color="primary"
                        variant="outlined"
                        sx={{ mt: 2, width: "100%" }}
                        onClick={_ => setPricing(calculateScreenshotPrice(options))}
                    >
                        {t('CalculatePricing')}
                    </Button>
                </Box>

            </CardContent>
        </Card>
    );
}
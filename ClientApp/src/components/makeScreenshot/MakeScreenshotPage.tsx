import { memo, useCallback, useEffect, useMemo, useState } from 'react';
import { useDispatch } from 'react-redux';
import { useTranslation } from 'react-i18next';
import {
    Container, Typography, Box, Tabs, Tab, Stack, Button, FormControlLabel, Switch,
    Grid
} from '@mui/material';
import TurtleIcon from '@mui/icons-material/Timer';
import RabbitIcon from '@mui/icons-material/DirectionsRun';
import FlashIcon from '@mui/icons-material/FlashOn';
import Alert from "@mui/material/Alert";
import * as Yup from 'yup';

import Form from '../form/Form';
import TextFieldWrapper from '../form/TextField';
import SelectFieldWrapper from '../form/SelectField';

import { getMakeScreenshotAction, getScreenshotAction } from '../../behavior/epic';
import { useAppSelector } from '../../behavior/rootReducer';

import {
    ScreenshotType, ScreenshotQualityMode, ClipModel, HeaderModel, CookieModel,
    ResourceBlockOptions, ColorSchemeOption, ScreenshotOptionsModel,
    ScreenshotState
} from '../../behavior/types';
import JsonPreview from './JsonPreview';
import ModalSection from './ModalSection';
import HighlightWordSection from './HighlightWordSection';
import AdvancedSection from './AdvancedSection';
import { timer } from 'rxjs';
import { FormikProps } from 'formik';
import { CostCalculation } from './CostCalculation';

export type FormClipModel = {
    width: number;
    height: number;
}

export enum Modes { Slow = 'Slow', Medium = 'Medium', Fast = 'Fast' }

const clipModels: Record<string, FormClipModel> = {
    "iPhone SE (375x667)": { width: 375, height: 667 },
    "iPhone 6/7/8 (375x667)": { width: 375, height: 667 },
    "iPhone X/XS/11 Pro (375x812)": { width: 375, height: 812 },
    "iPhone 12/13/14 (390x844)": { width: 390, height: 844 },
    "iPhone 14 Pro Max (430x932)": { width: 430, height: 932 },
    "Samsung Galaxy S8/S9/S10 (360x740)": { width: 360, height: 740 },
    "Galaxy S21 (360x800)": { width: 360, height: 800 },
    "Galaxy Fold (280x653)": { width: 280, height: 653 },
    "Pixel 4 (411x731)": { width: 411, height: 731 },
    "Pixel 7 (412x915)": { width: 412, height: 915 },
    "OnePlus 8 (412x915)": { width: 412, height: 915 },
    "iPad Mini (768x1024)": { width: 768, height: 1024 },
    "iPad (768x1024)": { width: 768, height: 1024 },
    "iPad Pro 10.5\" (834x1112)": { width: 834, height: 1112 },
    "iPad Pro 11\" (834x1194)": { width: 834, height: 1194 },
    "iPad Pro 12.9\" (1024x1366)": { width: 1024, height: 1366 },
    "Galaxy Tab S6 (800x1280)": { width: 800, height: 1280 },
    "Surface Pro 7 (912x1368)": { width: 912, height: 1368 },
    "Laptop 11\" (1366x768)": { width: 1366, height: 768 },
    "Laptop 13\" (1280x800)": { width: 1280, height: 800 },
    "Laptop 14\" (1440x900)": { width: 1440, height: 900 },
    "Laptop 15\" (1440x900)": { width: 1440, height: 900 },
    "Laptop 15.6\" FHD (1920x1080)": { width: 1920, height: 1080 },
    "Laptop 17\" (1920x1200)": { width: 1920, height: 1200 },
    "Desktop HD (1366x768)": { width: 1366, height: 768 },
    "Desktop Full HD (1920x1080)": { width: 1920, height: 1080 },
    "Desktop 2K (2560x1440)": { width: 2560, height: 1440 },
    "Desktop 4K (3840x2160)": { width: 3840, height: 2160 },
    "UltraWide 21:9 (2560x1080)": { width: 2560, height: 1080 },
    "UltraWide 32:9 (3840x1080)": { width: 3840, height: 1080 },
    "Mac Studio (3456x2234)": { width: 3456, height: 2234 },
    "iMac 27\" 5K (5120x2880)": { width: 5120, height: 2880 }
};

const mapModeToBackend = (mode: Modes): ScreenshotQualityMode => {
    switch (mode) {
        case Modes.Slow:
            return ScreenshotQualityMode.High;
        case Modes.Medium:
            return ScreenshotQualityMode.Medium;
        case Modes.Fast:
            return ScreenshotQualityMode.Low;
    }
};

// Utility to map selected strings to ResourceBlockOptions
const mapBlockResources = (values: string[]): ResourceBlockOptions => {
    const map: Record<string, ResourceBlockOptions> = {
        Images: ResourceBlockOptions.Images,
        Fonts: ResourceBlockOptions.Fonts,
        Media: ResourceBlockOptions.Media,
        Scripts: ResourceBlockOptions.Scripts,
        Stylesheets: ResourceBlockOptions.Stylesheets
    };
    return values.reduce<ResourceBlockOptions>((acc, r) => acc | (map[r] ?? ResourceBlockOptions.None), ResourceBlockOptions.None);
};

const toUtcISOString = (localDateTime?: string): string | undefined => {
    if (!localDateTime) return undefined;

    // "2026-03-22T15:30"
    const local = new Date(localDateTime);

    return new Date(
        local.getTime() - local.getTimezoneOffset() * 60000
    ).toISOString();
};

const formatFormValues = (values: ScreenshotFormValues, qualityTab: Modes) => {
    let clip: ClipModel | undefined = { ...values.clip };

    if (values.useFullHeight)
        clip.height = null;

    if (values.elementSelector)
        clip = undefined;

    const payload: ScreenshotOptionsModel = {
        url: values.url,
        screenshotType: values.screenshotType,
        mode: mapModeToBackend(qualityTab),
        clip,
        element: values.elementSelector ? { selector: values.elementSelector, clip: values.clip } : undefined,
        modalModel: values.modalEnabled ? values.modal : undefined,
        highlightWord: values.highlightEnabled ? values.highlightWord : undefined,
        advancedConfiguration: values.advancedEnabled ? {
            locale: values.locale,
            timezoneId: values.timezoneId,
            colorScheme: values.colorScheme,
            waitForSelector: values.waitForSelector || null,
            blockResources: mapBlockResources(values.blockResources),
            headers: values.headers,
            cookies: values.cookies.map(cookie => ({ ...cookie, expires: toUtcISOString(cookie.expires) }))
        } : undefined
    };

    return payload;
}

export type Modal = {
    dismissDialogs: boolean,
    hidePopups: boolean,
    hideSelectors: string[]
}

export type HighlightWord = {
    word: string;
    color: string;
}

export type ScreenshotFormValues = {
    url: string;
    screenshotType: ScreenshotType;
    preset: string;
    clip: FormClipModel;
    modalEnabled: boolean;
    modal: Modal
    useFullHeight: boolean;
    elementSelector: string;
    highlightEnabled: boolean;
    highlightWord: HighlightWord;
    advancedEnabled: boolean;
    locale: string;
    timezoneId: string;
    colorScheme: ColorSchemeOption;
    waitForSelector: string;
    blockResources: string[];
    headers: HeaderModel[];
    cookies: CookieModel[];
}

type ChangeTab = (_: any, newValue: Modes, setFieldValue: (name: string, value: any) => void) => void;

const ScreenshotForm = () => {
    const { t } = useTranslation();
    const dispatch = useDispatch();
    const screenshotData = useAppSelector(state => state.basic.screenshot);
    const [qualityTab, setQualityTab] = useState<Modes>(Modes.Medium);

    const screenshot = screenshotData?.screenshot;

    useEffect(() => {
        const lastChangedDate = screenshotData?.lastChangedDate
            ? new Date(screenshotData.lastChangedDate)
            : null;

        if (!lastChangedDate || !screenshot || screenshot?.state !== ScreenshotState.New)
            return;

        const subscriber = timer(200)
            .subscribe(_ => dispatch(getScreenshotAction(screenshot.id)))

        return () => subscriber.unsubscribe();
    }, [screenshotData?.lastChangedDate]);

    const initialValues = useMemo<ScreenshotFormValues>(() => ({
        url: '',
        screenshotType: ScreenshotType.Png,
        preset: "Desktop 4K (3840x2160)",
        clip: clipModels["Desktop 4K (3840x2160)"],
        useFullHeight: false,
        elementSelector: '',
        modalEnabled: false,
        modal: {
            dismissDialogs: false,
            hideSelectors: [],
            hidePopups: false,
        },
        highlightEnabled: false,
        highlightWord: {
            color: '#FFFF00',
            word: '',
        },
        advancedEnabled: false,
        locale: 'en-US',
        timezoneId: 'UTC',
        colorScheme: ColorSchemeOption.Light,
        waitForSelector: '',
        blockResources: [],
        headers: [],
        cookies: []
    }), []);

    const validationSchema = useMemo(() => Yup.object().shape({
        url: Yup.string().required('URL обовʼязковий').url('Невірний URL').max(2048),
        clip: Yup.object().shape({
            width: Yup.number().min(1).max(5000).required(),
            height: Yup.number().min(1).max(7000).required()
        }),
        headers: Yup.array().of(
            Yup.object().shape({ name: Yup.string().required(), value: Yup.string().required() })
        ).max(40),
        cookies: Yup.array().of(
            Yup.object().shape({ name: Yup.string().required(), value: Yup.string().required(), domain: Yup.string().required(), path: Yup.string().required() })
        ).max(20)
    }), []);

    const handleTabChange: ChangeTab = useCallback((_, newValue, setFieldValue) => {
        setQualityTab(newValue);
        if (newValue === Modes.Fast) {
            setFieldValue('useFullHeight', false);
            setFieldValue('preset', "Desktop 4K (3840x2160)");
            setFieldValue('clip', clipModels["Desktop 4K (3840x2160)"]);
        }
    }, []);

    return (
        <Container maxWidth="xl">
            <Typography variant="h4" align="center" gutterBottom>
                {t('MainPage.screenshotService')}
            </Typography>

            <Typography variant="body1" align="center" gutterBottom>
                {t('MainPage.captureManageScreenshots')}
            </Typography>

            <Form
                initialValues={initialValues}
                validationSchema={validationSchema}
                validateOnChange={false}
                onSubmit={(values) => {
                    const payload = formatFormValues(values, qualityTab);
                    dispatch(getMakeScreenshotAction(payload));
                }}
            >
                {(formContext) =>
                    <FormContent
                        formContext={formContext}
                        qualityTab={qualityTab}
                        handleTabChange={handleTabChange}
                    />
                }
            </Form>
        </Container>
    );
};

type FormContentProps<T> = {
    formContext: FormikProps<T>
    qualityTab: Modes
    handleTabChange: ChangeTab
}

const FormContent = memo(({ formContext: { values, setFieldValue }, qualityTab, handleTabChange }: FormContentProps<ScreenshotFormValues>) => {
    const payload = useMemo(
        () => formatFormValues(values, qualityTab),
        [values, qualityTab]
    );

    const { t } = useTranslation();

    const isFastMode = qualityTab === Modes.Fast;
    const isElementSet = !!values.elementSelector;

    const serverError = useAppSelector(state => state.basic.error);
    const screenshotData = useAppSelector(state => state.basic.screenshot);

    const screenshot = screenshotData?.screenshot;

    const error = !serverError && screenshot?.state === ScreenshotState.Failed
        ? 'Не вдалося створити скріншот'
        : serverError;

    return (
        <>
            {/* Quality Tabs */}
            <Box sx={{ mb: 3 }}>
                <Tabs value={qualityTab} onChange={(e, val) => handleTabChange(e, val, setFieldValue)} centered>
                    <Tab value={Modes.Slow} icon={<TurtleIcon />} label="Повільний / Висока якість" />
                    <Tab value={Modes.Medium} icon={<RabbitIcon />} label="Середня якість" />
                    <Tab value={Modes.Fast} icon={<FlashIcon />} label="Швидкий / Низька якість" />
                </Tabs>
            </Box>

            <Grid container spacing={3} alignItems="stretch" wrap="nowrap">
                <Grid order={-1} size={{ xs: 12, md: 3 }} sx={{ minWidth: { md: 320 }, maxWidth: { md: 400 }, flexShrink: 0 }}>
                    <CostCalculation options={payload} />
                </Grid>

                <Grid order={1} size={{ xs: 12, md: 3 }} sx={{ minWidth: { md: 320 }, maxWidth: { md: 400 }, flexShrink: 0 }}>
                    <Box sx={{ height: "100%" }}>
                        <JsonPreview values={payload} />
                    </Box>
                </Grid>

                <Grid size={{ xs: 12, md: 8 }} sx={{ flexGrow: 1, overflow: "auto", pt: 2 }}>
                    <Stack spacing={2} paddingLeft={5}>

                        {isElementSet && (
                            <Alert severity="warning">
                                Ви обрали скріншот по елементу. Буде захоплено лише вказаний елемент. Переконайтесь, що елемент існує на сторінці та знаходиться в межах розміру Width(px)×Height(px). Якщо хочете зробити повний скріншот — видаліть селектор.
                            </Alert>
                        )}

                        <TextFieldWrapper name="url" label="URL" fullWidth slotProps={{ htmlInput: { maxLength: 2048 } }} />

                        <SelectFieldWrapper
                            name="screenshotType"
                            label="Тип скріншоту"
                            options={[
                                { value: ScreenshotType.Png, label: 'PNG' },
                                { value: ScreenshotType.Jpeg, label: 'JPEG' }
                            ]}
                        />

                        <Typography variant="subtitle1">Оберіть Clip або Element</Typography>

                        <SelectFieldWrapper
                            name="preset"
                            label="Пресет екрану"
                            options={Object.keys(clipModels).map(label => ({ value: label, label }))}
                            onChange={e => {
                                setFieldValue('preset', e.target.value);
                                setFieldValue('clip', clipModels[e.target.value as string]);
                            }}
                        />

                        <FormControlLabel
                            control={
                                <Switch
                                    checked={values.useFullHeight}
                                    onChange={e => setFieldValue('useFullHeight', e.target.checked)}
                                    disabled={!!values.elementSelector || isFastMode}
                                    name="useFullHeight"
                                />
                            }
                            label="Повна висота (захоплює всю сторінку)"
                        />

                        <Stack direction="row" spacing={2}>
                            <TextFieldWrapper name="clip.width" label="Ширина (px)" type="number" />
                            <TextFieldWrapper name="clip.height" label="Висота (px)" type="number" disabled={values.useFullHeight} />
                        </Stack>

                        <TextFieldWrapper
                            name="elementSelector"
                            label="Або CSS селектор елемента"
                            placeholder="Елемент має бути в межах 5000x7000 px"
                            fullWidth
                            onChange={e => {
                                if (e.target.value)
                                    setFieldValue('useFullHeight', false);

                                setFieldValue('elementSelector', e.target.value);
                            }}
                            slotProps={{ htmlInput: { maxLength: 200 } }}
                        />

                        {/* Modal Section */}
                        <ModalSection modalEnabled={values.modalEnabled} />

                        {/* Highlight Word Section */}
                        <HighlightWordSection highlightEnabled={values.highlightEnabled} />

                        {/* Advanced Configuration */}
                        <AdvancedSection advancedEnabled={values.advancedEnabled} />

                        {error && <Typography color="error">{error}</Typography>}

                        <Button type="submit" variant="contained" color="primary">
                            {t('MainPage.getScreenshot')}
                        </Button>

                        {screenshot?.state === ScreenshotState.Successful && (
                            <>
                                <Stack direction="row" spacing={2}>
                                    <Button
                                        variant="contained"
                                        color="success"
                                        onClick={async () => {
                                            const response = await fetch(screenshot.url, { mode: "cors" });
                                            const blob = await response.blob();
                                            const blobUrl = window.URL.createObjectURL(blob);

                                            const link = document.createElement("a");
                                            link.href = blobUrl;

                                            const extension = values.screenshotType?.toLowerCase() ?? "png";
                                            link.download = `screenshot-${screenshot.id}.${extension}`;

                                            document.body.appendChild(link);
                                            link.click();
                                            document.body.removeChild(link);

                                            window.URL.revokeObjectURL(blobUrl);
                                        }}
                                    >
                                        Завантажити скріншот
                                    </Button>
                                </Stack>

                                <Box textAlign="center" mt={4}>
                                    <Typography variant="h6">Скріншот:</Typography>

                                    <Box
                                        component="img"
                                        src={screenshot.url}
                                        alt="Скріншот"
                                        sx={{ maxWidth: '100%', cursor: 'pointer', mt: 1 }}
                                        onClick={() => window.open(screenshot.url, '_blank')}
                                    />
                                </Box>
                            </>
                        )}
                    </Stack>
                </Grid>
            </Grid>
        </>
    );
});

export default ScreenshotForm;
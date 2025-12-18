import React, { useEffect, useState } from 'react';
import { Clip, ScreenshotType } from "../behavior/types";
import { useAppSelector } from "../behavior/rootReducer";
import { useDispatch } from "react-redux";
import { getMakeScreenshotAction } from "../behavior/epic";
import { useTranslation } from 'react-i18next';
import {
    Container,
    Typography,
    Box,
    Tabs,
    Tab,
    Stack,
    Button,
    Accordion,
    AccordionSummary,
    AccordionDetails,
} from '@mui/material';
import TurtleIcon from '@mui/icons-material/Timer';
import RabbitIcon from '@mui/icons-material/DirectionsRun';
import FlashIcon from '@mui/icons-material/FlashOn';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import * as Yup from 'yup';
import Form from './form/Form';

import TextFieldWrapper from './form/TextField';
import SelectFieldWrapper from './form/SelectField';
import SwitchFieldWrapper from './form/SwitchField';
import ArrayFieldWrapper from './form/ArrayField';

const $4kClipName = "Desktop 4K (3840x2160)";

const clipModels: Record<string, Clip> = {
    // Mobile
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

    // Tablets
    "iPad Mini (768x1024)": { width: 768, height: 1024 },
    "iPad (768x1024)": { width: 768, height: 1024 },
    "iPad Pro 10.5\" (834x1112)": { width: 834, height: 1112 },
    "iPad Pro 11\" (834x1194)": { width: 834, height: 1194 },
    "iPad Pro 12.9\" (1024x1366)": { width: 1024, height: 1366 },
    "Galaxy Tab S6 (800x1280)": { width: 800, height: 1280 },

    // Laptops
    "Surface Pro 7 (912x1368)": { width: 912, height: 1368 },
    "Laptop 11\" (1366x768)": { width: 1366, height: 768 },
    "Laptop 13\" (1280x800)": { width: 1280, height: 800 },
    "Laptop 14\" (1440x900)": { width: 1440, height: 900 },
    "Laptop 15\" (1440x900)": { width: 1440, height: 900 },
    "Laptop 15.6\" FHD (1920x1080)": { width: 1920, height: 1080 },
    "Laptop 17\" (1920x1200)": { width: 1920, height: 1200 },

    // Desktop
    "Desktop HD (1366x768)": { width: 1366, height: 768 },
    "Desktop Full HD (1920x1080)": { width: 1920, height: 1080 },
    "Desktop 2K (2560x1440)": { width: 2560, height: 1440 },
    "Desktop 4K (3840x2160)": { width: 3840, height: 2160 },
    "UltraWide 21:9 (2560x1080)": { width: 2560, height: 1080 },
    "UltraWide 32:9 (3840x1080)": { width: 3840, height: 1080 },
    "Mac Studio (3456x2234)": { width: 3456, height: 2234 },
    "iMac 27\" 5K (5120x2880)": { width: 5120, height: 2880 }
};

enum Modes {
    Slow = 'Slow',
    Medium = 'Medium',
    Fast = 'Fast'
}

type Header = { name: string; value: string };

const ScreenshotForm = () => {
    const { t } = useTranslation();
    const dispatch = useDispatch();
    const { error: serverError, image } = useAppSelector(state => state);

    const [qualityTab, setQualityTab] = useState<Modes>(Modes.Medium);

    const initialValues = {
        url: '',
        screenshotType: ScreenshotType.Png,
        preset: $4kClipName,
        clip: clipModels[$4kClipName],
        useFullHeight: false,
        blockBanners: false,
        headers: [] as Header[]
    };

    const validationSchema = Yup.object().shape({
        url: Yup.string().required(t('Validation.required', { field: t('MainPage.url') })).url(t('Validation.url', { field: t('MainPage.url') })),
        clip: Yup.object().shape({
            width: Yup.number()
                .required(t('Validation.required', { field: t('MainPage.widthPx') }))
                .min(1, t('Validation.minValue', { field: t('MainPage.widthPx'), min: 1 }))
                .max(10000, t('Validation.maxValue', { field: t('MainPage.widthPx'), max: 10000 })),
            height: Yup.number()
                .required(t('Validation.required', { field: t('MainPage.heightPx') }))
                .min(1, t('Validation.minValue', { field: t('MainPage.heightPx'), min: 1 }))
                .max(10000, t('Validation.maxValue', { field: t('MainPage.heightPx'), max: 10000 })),
        }),
        headers: Yup.array().of(
            Yup.object().shape({
                name: Yup.string().required(t('Validation.required', { field: t('MainPage.headerTitle') })),
                value: Yup.string().required(t('Validation.required', { field: t('MainPage.headerValue') }))
            })
        )
    });

    const handleTabChange = (_: React.SyntheticEvent, newValue: Modes, setFieldValue: any) => {
        setQualityTab(newValue);
        if (newValue === Modes.Fast) {
            setFieldValue('useFullHeight', false);
            setFieldValue('preset', $4kClipName);
            setFieldValue('clip', clipModels[$4kClipName]);
        }
    };

    return (
        <Container maxWidth="md">
            <Typography variant="h4" align="center" gutterBottom>{t('MainPage.screenshotService')}</Typography>
            <Typography variant="body1" align="center" gutterBottom>{t('MainPage.captureManageScreenshots')}</Typography>
            <Form
                initialValues={initialValues}
                validationSchema={validationSchema}
                onSubmit={(values) => {
                    dispatch(getMakeScreenshotAction({
                        url: values.url,
                        screenshotType: values.screenshotType,
                        clip: values.useFullHeight ? { ...values.clip, height: null } : values.clip
                    }));
                }}
            >
                {({ values, setFieldValue }) => {
                    const isFastMode = qualityTab === Modes.Fast;
                    return (
                        <>
                            <Box sx={{ mb: 3 }}>
                                <Tabs value={qualityTab} onChange={(e, val) => handleTabChange(e, val, setFieldValue)} centered>
                                    <Tab value={Modes.Slow} icon={<TurtleIcon />} label={t('MainPage.slowHighQuality')} />
                                    <Tab value={Modes.Medium} icon={<RabbitIcon />} label={t('MainPage.mediumQuality')} />
                                    <Tab value={Modes.Fast} icon={<FlashIcon />} label={t('MainPage.fastLowQuality')} />
                                </Tabs>
                            </Box>
                            <Stack spacing={2}>
                                <TextFieldWrapper name="url" label={t('MainPage.url')} placeholder={t('MainPage.urlPlaceholder')} fullWidth />
                                <SelectFieldWrapper
                                    name="screenshotType"
                                    label={t('MainPage.screenshotType')}
                                    options={[
                                        { value: ScreenshotType.Png, label: t('MainPage.png') },
                                        { value: ScreenshotType.Jpeg, label: t('MainPage.jpeg') }
                                    ]}
                                />
                                <SelectFieldWrapper
                                    name="preset"
                                    label={t('MainPage.screenPreset')}
                                    options={Object.keys(clipModels).map(label => ({ value: label, label }))}
                                    onChange={(e) => {
                                        const val = e.target.value;
                                        setFieldValue('preset', val);
                                        setFieldValue('clip', clipModels[val]);
                                    }}
                                />
                                {!isFastMode && <SwitchFieldWrapper name="useFullHeight" label={t('MainPage.useFullHeight')} />}
                                <Stack direction="row" spacing={2}>
                                    <TextFieldWrapper
                                        name="clip.width"
                                        label={t('MainPage.widthPx')}
                                        type="number"
                                        disabled={isFastMode}
                                    />
                                    <TextFieldWrapper
                                        name="clip.height"
                                        label={t('MainPage.heightPx')}
                                        type="number"
                                        disabled={values.useFullHeight || isFastMode}
                                    />
                                </Stack>

                                <Accordion>
                                    <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                                        <Typography variant="subtitle1">{t('MainPage.additionalSettings')}</Typography>
                                    </AccordionSummary>
                                    <AccordionDetails>
                                        <Stack spacing={2}>
                                            <SwitchFieldWrapper name="blockBanners" label={t('MainPage.blockBanners')} />
                                            <ArrayFieldWrapper<Header> name="headers" label={t('MainPage.headersTitle')} emptyValue={{ name: '', value: '' }}>
                                                {({ index, parentName }) => (
                                                    <Stack direction="row" key={index} spacing={1} alignItems="top" sx={{ width: '100%' }}>
                                                        <TextFieldWrapper
                                                            name={`${parentName}.[${index}].name`}
                                                            label={t("MainPage.headerTitle")}
                                                            fullWidth
                                                        />
                                                        <TextFieldWrapper
                                                            name={`${parentName}.[${index}].value`}
                                                            label={t("MainPage.headerValue")}
                                                            fullWidth
                                                        />
                                                    </Stack>
                                                )}
                                            </ArrayFieldWrapper>
                                        </Stack>
                                    </AccordionDetails>
                                </Accordion>


                                {serverError && <Typography color="error">{serverError}</Typography>}

                                <Stack direction="row" spacing={2}>
                                    <Button type="submit" variant="contained" color="primary">{t('MainPage.getScreenshot')}</Button>
                                    {values.url && image && (
                                        <Button
                                            variant="contained"
                                            color="success"
                                            onClick={() => {
                                                const link = document.createElement('a');
                                                link.download = `screenshot.${values.screenshotType.toLowerCase()}`;
                                                link.href = image;
                                                link.click();
                                            }}
                                        >
                                            {t('MainPage.downloadScreenshot')}
                                        </Button>
                                    )}
                                </Stack>

                                {image && (
                                    <Box textAlign="center" mt={4}>
                                        <Typography variant="h6">{t('MainPage.screenshot')}:</Typography>
                                        <Box
                                            component="img"
                                            src={image}
                                            alt="Screenshot"
                                            sx={{ maxWidth: '100%', cursor: 'pointer', mt: 1 }}
                                            onClick={() => window.open(image, '_blank')}
                                        />
                                    </Box>
                                )}
                            </Stack>
                        </>
                    );
                }}
            </Form>
        </Container>
    );
};

export default ScreenshotForm;

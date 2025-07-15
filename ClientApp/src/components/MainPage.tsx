import { Container, Button, Form, Row, Col } from "react-bootstrap";
import React, { useEffect, useState } from 'react';
import { Clip, ScreenshotType } from "../behavior/types";
import { useAppSelector } from "../behavior/rootReducer";
import { useDispatch } from "react-redux";
import { getMakeScreenshotAction } from "../behavior/epic";
import { useTranslation } from 'react-i18next';

const $4kClipName = "Desktop 4K (3840x2160)";

const clipModels: Record<string, Clip> = {
    "iPhone SE (375x667)": { width: 375, height: 667 },
    "iPhone 12 (390x844)": { width: 390, height: 844 },
    "iPhone 14 Pro Max (430x932)": { width: 430, height: 932 },
    "iPad Mini (768x1024)": { width: 768, height: 1024 },
    "iPad Pro 11\" (834x1194)": { width: 834, height: 1194 },
    "Galaxy S21 (360x800)": { width: 360, height: 800 },
    "Galaxy Fold (280x653)": { width: 280, height: 653 },
    "Pixel 7 (412x915)": { width: 412, height: 915 },
    "Surface Pro 7 (912x1368)": { width: 912, height: 1368 },
    "Laptop 13\" (1280x800)": { width: 1280, height: 800 },
    "Laptop 15\" (1440x900)": { width: 1440, height: 900 },
    "Desktop HD (1366x768)": { width: 1366, height: 768 },
    "Desktop Full HD (1920x1080)": { width: 1920, height: 1080 },
    "Desktop 4K (3840x2160)": { width: 3840, height: 2160 }
};

export default () => {
    const [url, setUrl] = useState('');
    const [screenshotType, setScreenshotType] = useState<ScreenshotType>(ScreenshotType.Png);
    const [clip, setClip] = useState<Clip>(clipModels[$4kClipName]);
    const [preset, setPreset] = useState($4kClipName);
    const [useFullHeight, setUseFullHeight] = useState(false);
    const [error, setError] = useState('');
    const { error: serverError, image } = useAppSelector(state => state);
    const dispatch = useDispatch();
    const { t } = useTranslation();

    useEffect(() => {
        setError(serverError ?? '');
    }, [serverError]);

    const validateUrl = (url: string) => {
        const urlPattern = new RegExp('^(https?:\\/\\/)' + // protocol
            '((([a-z\\d]([a-z\\d-]*[a-z\\d])*)\\.?)+[a-z]{2,}|' + // domain
            '((\\d{1,3}\\.){3}\\d{1,3}))' + // or IP
            '(\\:\\d+)?(\\/[-a-z\\d%_.~+]*)*' + // port/path
            '(\\?[;&a-z\\d%_.~+=-]*)?' + // query
            '(\\#[-a-z\\d_]*)?$', 'i');
        return urlPattern.test(url);
    };

    const handlePresetChange = (presetLabel: string) => {
        setPreset(presetLabel);
        const selected = clipModels[presetLabel];
        setClip({ width: selected.width, height: selected.height });
    };

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        if (!validateUrl(url)) {
            setError(t('MainPage.enterValidURL'));
            return;
        }
        if (!clip.width || clip.width < 1 || clip.width > 10000 || (clip.height !== null && (clip.height < 1 || clip.height > 10000))) {
            setError(t('MainPage.enterValidDimensions'));
            return;
        }

        dispatch(getMakeScreenshotAction({
            url,
            screenshotType,
            clip: useFullHeight ? { ...clip, height: null } : clip,
        }));
    };

    return (
        <Container className="mt-4">
            <h2 className="text-center">{t('MainPage.screenshotService')}</h2>
            <p className="text-center">{t('MainPage.captureManageScreenshots')}</p>
            <Form onSubmit={handleSubmit}>

                <Form.Group controlId="formUrl" className="mb-3">
                    <Form.Label>{t('MainPage.url')}</Form.Label>
                    <Form.Control
                        type="text"
                        placeholder={t('MainPage.urlPlaceholder')}
                        value={url}
                        onChange={(e) => setUrl(e.target.value)}
                    />
                </Form.Group>

                <Form.Group controlId="formScreenshotType" className="mb-3">
                    <Form.Label>{t('MainPage.screenshotType')}</Form.Label>
                    <Form.Select
                        value={screenshotType}
                        onChange={(e) => setScreenshotType(e.target.value as ScreenshotType)}
                    >
                        <option value={ScreenshotType.Png}>{t('MainPage.png')}</option>
                        <option value={ScreenshotType.Jpeg}>{t('MainPage.jpeg')}</option>
                    </Form.Select>
                </Form.Group>

                <Form.Group controlId="formPreset" className="mb-3">
                    <Form.Label>{t('MainPage.screenPreset')}</Form.Label>
                    <Form.Select
                        value={preset}
                        onChange={(e) => handlePresetChange(e.target.value)}
                    >
                        {Object.keys(clipModels).map(label => (
                            <option key={label} value={label}>{label}</option>
                        ))}
                    </Form.Select>
                </Form.Group>

                <Form.Group controlId="formFullHeight" className="mb-3">
                    <Form.Check
                        type="checkbox"
                        label={t('MainPage.useFullHeight')}
                        checked={useFullHeight}
                        onChange={(e) => setUseFullHeight(e.target.checked)}
                    />
                </Form.Group>

                <Row className="mb-3">
                    <Col>
                        <Form.Label>{t('MainPage.widthPx')}</Form.Label>
                        <Form.Control
                            type="number"
                            min={1}
                            max={10000}
                            value={clip.width}
                            onChange={(e) => setClip({ ...clip, width: Number(e.target.value) })}
                        />
                    </Col>
                    <Col>
                        <Form.Label>{t('MainPage.heightPx')}</Form.Label>
                        <Form.Control
                            type="number"
                            min={1}
                            max={10000}
                            value={clip.height ?? ''}
                            disabled={useFullHeight}
                            onChange={(e) => setClip({ ...clip, height: Number(e.target.value) })}
                        />
                    </Col>
                </Row>

                {error && <Form.Text className="text-danger">{error}</Form.Text>}

                <Button variant="primary" type="submit">{t('MainPage.getScreenshot')}</Button>
                {image && (
                    <Button variant="success" className="ms-3" onClick={() => {
                        const link = document.createElement('a');
                        link.download = `screenshot.${screenshotType.toLowerCase()}`;
                        link.href = image;
                        link.click();
                    }}>
                        {t('MainPage.downloadScreenshot')}
                    </Button>
                )}
            </Form>

            {image && (
                <div className="mt-4 text-center">
                    <h3>{t('MainPage.screenshot')}:</h3>
                    <img
                        src={image}
                        alt="Screenshot"
                        className="screenshot-thumbnail cursor-pointer"
                        onClick={() => window.open(image, '_blank')}
                    />
                </div>
            )}
        </Container>
    );
};

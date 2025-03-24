import { Container, Button, Form, Row, Col } from "react-bootstrap";
import React, { useEffect, useState } from 'react';
import { Clip, ScreenshotType } from "../behavior/types";
import { useAppSelector } from "../behavior/rootReducer";
import { useDispatch } from "react-redux";
import { getMakeScreenshotAction } from "../behavior/epic";

export default () => {
    const [url, setUrl] = useState('');
    const [screenshotType, setScreenshotType] = useState<ScreenshotType>(ScreenshotType.Png);
    const [quality, setQuality] = useState<number>(0);
    const [fullScreen, setFullScreen] = useState<boolean | null>(null);
    const [clip, setClip] = useState<Clip | null>(null);
    const [error, setError] = useState('');
    const { error: serverError, image } = useAppSelector(state => state);
    const dispatch = useDispatch();

    useEffect(() => {
        setError(serverError ?? '');
    }, [serverError])

    const validateUrl = (url: string) => {
        const urlPattern = new RegExp('^(https?:\\/\\/)?' + // protocol
            '((([a-z\\d]([a-z\\d-]*[a-z\\d])*)\\.?)+[a-z]{2,}|' + // domain name
            '((\\d{1,3}\\.){3}\\d{1,3}))' + // OR ip (v4) address
            '(\\:\\d+)?(\\/[-a-z\\d%_.~+]*)*' + // port and path
            '(\\?[;&a-z\\d%_.~+=-]*)?' + // query string
            '(\\#[-a-z\\d_]*)?$', 'i'); // fragment locator
        return !!urlPattern.test(url);
    };

    const handleDownload = () => {
        if (image) {
            var link = document.createElement("a");
            link.download = `screenshot.${screenshotType.toLowerCase()}`;
            link.href = image;
            link.click();
        }
    };

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        if (!validateUrl(url)) {
            setError('Please enter a valid URL.');
            return;
        }

        if (!fullScreen && clip) {
            if (!clip.x || !clip.y || !clip.width || !clip.height) {
                setError('Please enter valid clip values.');
                return;
            }
        }

        const data = {
            url,
            screenshotType,
            quality: screenshotType === ScreenshotType.Jpeg ? quality : null,
            fullScreen: fullScreen,
            clip: fullScreen ? null : clip
        };

        dispatch(getMakeScreenshotAction(data));
    };

    return (
        <Container className="mt-4">
            <h2 className="text-center">Screenshot Service</h2>
            <p className="text-center">Capture and manage screenshots easily.</p>
            <Form onSubmit={handleSubmit} className="screenshot-form">
                <Form.Group controlId="formUrl" className="mb-3">
                    <Form.Label>Enter URL</Form.Label>
                    <Form.Control
                        type="text"
                        placeholder="Enter URL"
                        value={url}
                        onChange={(e) => setUrl(e.target.value)}
                    />
                </Form.Group>
                <Form.Group controlId="formScreenshotType" className="mb-3">
                    <Form.Label>Screenshot Type</Form.Label>
                    <Form.Control
                        as="select"
                        value={screenshotType}
                        onChange={(e) => setScreenshotType(e.target.value as ScreenshotType)}
                    >
                        <option value={ScreenshotType.Png}>PNG</option>
                        <option value={ScreenshotType.Jpeg}>JPEG</option>
                    </Form.Control>
                </Form.Group>
                {screenshotType === ScreenshotType.Jpeg && (
                    <Form.Group controlId="formQuality" className="mb-3">
                        <Form.Label>Quality (0-100)</Form.Label>
                        <Form.Control
                            type="range"
                            min="0"
                            max="100"
                            value={quality || 0}
                            onChange={(e) => setQuality(Number(e.target.value))}
                            placeholder="Enter quality (0-100)"
                        />
                        <Form.Text className="text-muted">Current Quality: {quality}</Form.Text>
                    </Form.Group>
                )}
                <Form.Group controlId="formFullScreen" className="mb-3">
                    <Form.Check
                        type="checkbox"
                        label="Full Screen"
                        checked={!!fullScreen}
                        onChange={(e) => setFullScreen(e.target.checked)}
                    />
                </Form.Group>
                {!fullScreen && (
                    <Form.Group controlId="formClip" className="mb-3">
                        <Row>
                            <Col>
                                <Form.Label>X</Form.Label>
                                <Form.Control
                                    type="number"
                                    placeholder="X coordinate"
                                    value={clip?.x || ''}
                                    max={10000}
                                    min={1}
                                    onChange={(e) => setClip({ ...clip, x: Number(e.target.value) })}
                                />
                            </Col>
                            <Col>
                                <Form.Label>Y</Form.Label>
                                <Form.Control
                                    type="number"
                                    placeholder="Y coordinate"
                                    value={clip?.y || ''}
                                    max={10000}
                                    min={1}
                                    onChange={(e) => setClip({ ...clip, y: Number(e.target.value) })}
                                />
                            </Col>
                        </Row>
                        <Row className="mt-2">
                            <Col>
                                <Form.Label>Width (px)</Form.Label>
                                <Form.Control
                                    type="number"
                                    placeholder="Width in pixels"
                                    value={clip?.width || ''}
                                    max={10000}
                                    min={1}
                                    onChange={(e) => setClip({ ...clip, width: Number(e.target.value) })}
                                />
                            </Col>
                            <Col>
                                <Form.Label>Height (px)</Form.Label>
                                <Form.Control
                                    type="number"
                                    placeholder="Height in pixels"
                                    value={clip?.height || ''}
                                    max={10000}
                                    min={1}
                                    onChange={(e) => setClip({ ...clip, height: Number(e.target.value) })}
                                />
                            </Col>
                        </Row>
                    </Form.Group>
                )}
                {error && <Form.Text className="text-danger">{error}</Form.Text>}
                <Button variant="primary" type="submit" className="mt-3">Get Screenshot</Button>
                {image && (
                    <Button variant="success" onClick={handleDownload} className="mt-3 ms-3">Download Screenshot</Button>
                )}
            </Form>
            {image && (
                <div className="mt-4 text-center">
                    <h3>Screenshot:</h3>
                    <img src={image}
                        alt="Screenshot"
                        className="screenshot-image screenshot-thumbnail cursor-pointer"
                        onClick={() => window.open(image, '_blank')}
                    />
                </div>
            )}
        </Container>
    );
};
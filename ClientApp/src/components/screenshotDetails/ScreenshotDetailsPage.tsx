import { useEffect } from "react";
import {
    Box,
    Card,
    CardContent,
    Typography,
    Chip,
    Skeleton,
    Stack
} from "@mui/material";
import { useParams } from "react-router-dom";
import { useDispatch } from "react-redux";
import { getScreenshotAction } from "../../behavior/epic";
import LazyImage from "../LazyImage";
import { setScreenshot } from "../../behavior/reducer";
import { useAppSelector } from "../../behavior/rootReducer";
import { isFailed, isSuccess, isLoading, getStateColor } from "../helpers";
import WebsiteLink from "../WebsiteLink";

const ScreenshotDetailsPage = () => {
    const { id } = useParams();
    const dispatch = useDispatch();

    const screenshot = useAppSelector((state) => state.basic.screenshot?.screenshot);
    const loaded = useAppSelector((state) => state.basic.loaded);

    const loading = !loaded || !screenshot || screenshot.id !== id;

    useEffect(() => {
        return () => void dispatch(setScreenshot({ data: null, error: null }));
    }, []);

    useEffect(() => {
        if (id && screenshot?.id !== id)
            dispatch(getScreenshotAction(id));
    }, [id, screenshot?.id]);

    const imagePlaceholder = (
        <Skeleton
            variant="rectangular"
            height={400}
            animation="wave"
        />
    );

    if (loading) {
        return (
            <Card sx={{ borderRadius: 3 }}>

                {/* Image placeholder */}
                {imagePlaceholder}

                <CardContent>

                    {/* website */}
                    <Skeleton width="60%" height={20} animation="wave" />

                    {/* title */}
                    <Skeleton
                        width="40%"
                        height={32}
                        animation="wave"
                        sx={{ mt: 2 }}
                    />

                    {/* description */}
                    <Skeleton width="100%" height={20} animation="wave" />
                    <Skeleton width="90%" height={20} animation="wave" />
                    <Skeleton width="85%" height={20} animation="wave" />

                    {/* chips row */}
                    <Stack direction="row" spacing={1} mt={3}>
                        <Skeleton variant="rounded" width={70} height={28} animation="wave" />
                        <Skeleton variant="rounded" width={90} height={28} animation="wave" />
                    </Stack>

                    {/* date */}
                    <Box mt={3}>
                        <Skeleton width={120} height={20} animation="wave" />
                    </Box>

                </CardContent>
            </Card>
        );
    }

    return (
        <Box p={2}>
            <Card sx={{ borderRadius: 3 }}>
                {isSuccess(screenshot.state) &&
                    <LazyImage
                        src={screenshot.url}
                        alt={screenshot.title ?? "Знімок екрана"}
                        height={400}
                    />}

                {isFailed(screenshot.state) &&
                    <Box
                        height={400}
                        display="flex"
                        alignItems="center"
                        justifyContent="center"
                        sx={{
                            backgroundColor: "error.light",
                            color: "error.contrastText"
                        }}
                    >
                        <Typography variant="h6">
                            Не вдалося завантажити знімок екрана
                        </Typography>
                    </Box>
                }

                {isLoading(screenshot.state) && imagePlaceholder}

                <CardContent>

                    <Box mt={2}>
                        <Typography variant="caption">
                            <WebsiteLink url={screenshot.websiteUrl} />
                        </Typography>
                    </Box>

                    <Typography variant="h5">
                        {screenshot.title ?? "Без назви"}
                    </Typography>

                    <Typography variant="body2" color="text.secondary">
                        {screenshot.description ?? "Опис відсутній"}
                    </Typography>

                    <Stack direction="row" mt={2}>
                        <Chip
                            label={screenshot.state}
                            color={getStateColor(screenshot.state)}
                            size="small"
                        />
                    </Stack>

                    <Stack direction="row" mt={1}>
                        <Chip
                            label={screenshot.type}
                            size="small"
                            variant="outlined"
                        />
                    </Stack>

                    <Box display="flex" flexWrap="wrap" mt={1} gap={1}>
                        {screenshot.categories.map((cat) => (
                            <Chip
                                key={cat.id}
                                label={cat.name}
                                size="small"
                                sx={{
                                    backgroundColor: cat.color,
                                    color: "#ffffff"
                                }}
                            />
                        ))}
                    </Box>

                    <Box mt={2}>
                        <Typography variant="caption">
                            {new Date(screenshot.createdAt).toLocaleString()}
                        </Typography>
                    </Box>

                </CardContent>
            </Card>
        </Box>
    );
};

export default ScreenshotDetailsPage;
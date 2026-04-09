import { useEffect, useMemo } from "react";
import {
    Box,
    Grid,
    Pagination,
    Typography,
    Card,
    CardContent,
    Skeleton
} from "@mui/material";
import { useDispatch } from "react-redux";
import { useAppSelector } from "../../behavior/rootReducer";
import { getCategoriesAction, getScreenshotsAction } from "../../behavior/epic";
import { Screenshot, SearchScope } from "../../behavior/types";
import Form from "../form/Form";
import ScreenshotFiltersForm from "./ScreenshotFiltersForm";
import { ScreenshotCard } from "./ScreenshotCard";
import { setScreenshot, setScreenshots } from "../../behavior/reducer";
import { DEFAULT_PAGE_SIZE, formatQueryPaging } from "../../behavior/utils/search";
import { useNavigate } from "react-router-dom";

type FilterForm = {
    query: string;
    searchScope: SearchScope;
    categoryIds: string[];
};

const ScreenshotsPage = () => {
    const dispatch = useDispatch();
    const { screenshots, categories, loaded } = useAppSelector(s => s);
    const navigate = useNavigate();
    const handleClick = (s: Screenshot) => {
        dispatch(setScreenshot({ data: s, error: null }));
        navigate(`/screenshot/${s.id}`);
    };

    const nextAmountOfItems = screenshots?.pageSize ?? DEFAULT_PAGE_SIZE
    useEffect(() => {
        dispatch(getCategoriesAction());
        dispatch(getScreenshotsAction());

        return () => void dispatch(setScreenshots({ data: null, error: null }))
    }, []);

    const initialValues = useMemo((): FilterForm => {
        const queryPaging = formatQueryPaging();
        return {
            searchScope: queryPaging.searchScope,
            categoryIds: queryPaging.categoryIds ?? [],
            query: queryPaging.query ?? ''
        }
    }, []);

    return (
        <Box p={3}>
            <Typography variant="h4" mb={2}>
                Screenshots
            </Typography>

            <Form<FilterForm>
                initialValues={initialValues}
                onSubmit={(values) => {
                    dispatch(getScreenshotsAction({
                        page: 1,
                        pageSize: DEFAULT_PAGE_SIZE,
                        ...values
                    }));
                }}
            >
                {(formik) => {

                    const handleSubmit = () => {
                        formik.submitForm();
                    };

                    const handlePageChange = (_: any, page: number) => {
                        dispatch(getScreenshotsAction({
                            page,
                            pageSize: DEFAULT_PAGE_SIZE,
                            ...formik.values
                        }));
                    };

                    return (
                        <>
                            <ScreenshotFiltersForm
                                formik={formik}
                                categories={categories?.items}
                                onSubmit={handleSubmit}
                            />

                            {!loaded ? (
                                <Grid container spacing={2}>
                                    {Array.from({ length: nextAmountOfItems }).map((_, i) => (
                                        <Grid key={i} size={{ xs: 12, md: 4 }}>
                                            <Card sx={{ borderRadius: 3 }}>
                                                <CardContent>

                                                    {/* website */}
                                                    <Skeleton width="80%" height={20} animation="wave" />

                                                    {/* meta row */}
                                                    <Box
                                                        mt={1}
                                                        display="flex"
                                                        justifyContent="space-between"
                                                        alignItems="center"
                                                    >
                                                        <Skeleton variant="rounded" width={60} height={24} animation="wave" />
                                                        <Skeleton width={80} height={20} animation="wave" />
                                                    </Box>

                                                    {/* type */}
                                                    <Box mt={1}>
                                                        <Skeleton variant="rounded" width={50} height={24} animation="wave" />
                                                    </Box>

                                                    {/* title */}
                                                    <Skeleton
                                                        width="65%"
                                                        height={28}
                                                        animation="wave"
                                                        sx={{ mt: 2 }}
                                                    />

                                                    {/* description */}
                                                    <Skeleton width="100%" height={20} animation="wave" />
                                                    <Skeleton width="85%" height={20} animation="wave" />

                                                </CardContent>
                                            </Card>
                                        </Grid>
                                    ))}
                                </Grid>
                            ) : (
                                <Grid container spacing={2}>
                                    {screenshots?.items.map((s) => (
                                        <Grid key={s.id} size={{ xs: 12, md: 4 }}>
                                            <div onClick={() => handleClick(s)} style={{ cursor: 'pointer' }}>
                                                <ScreenshotCard s={s} />
                                            </div>
                                        </Grid>
                                    ))}
                                </Grid>
                            )}

                            <Box mt={3} display="flex" justifyContent="center">
                                <Pagination
                                    count={Math.ceil((screenshots?.total ?? 0) / DEFAULT_PAGE_SIZE)}
                                    onChange={handlePageChange}
                                />
                            </Box>
                        </>
                    );
                }}
            </Form>
        </Box>
    );
};

export default ScreenshotsPage;
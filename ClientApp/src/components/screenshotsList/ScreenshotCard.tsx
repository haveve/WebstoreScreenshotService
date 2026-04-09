import { Card, CardContent, Typography, Box, Chip } from "@mui/material";
import { Screenshot } from "../../behavior/types";
import { getStateColor } from "../helpers";
import WebsiteLink from "../WebsiteLink";

const formatDate = (date: string) => {
    return new Date(date).toLocaleString();
};

export const ScreenshotCard = ({ s }: { s: Screenshot }) => {
    return (
        <Card sx={{ borderRadius: 3 }}>
            <CardContent>
                <Typography
                    variant="subtitle2"
                    color="text.secondary"
                    noWrap
                >
                    <WebsiteLink url={s.websiteUrl} />
                </Typography>

                <Box
                    mt={1}
                    display="flex"
                    justifyContent="space-between"
                    alignItems="center"
                >
                    <Chip
                        label={s.state}
                        color={getStateColor(s.state)}
                        size="small"
                    />

                    <Typography variant="caption" color="text.secondary">
                        {formatDate(s.createdAt)}
                    </Typography>
                </Box>

                <Box mt={1}>
                    <Chip
                        label={s.type}
                        variant="outlined"
                        size="small"
                    />
                </Box>

                <Typography variant="h6" mt={2}>
                    {s.title ?? "No title"}
                </Typography>

                <Typography variant="body2" color="text.secondary">
                    {s.description ?? "No description"}
                </Typography>

                <Box display="flex" flexWrap="wrap" mt={1} gap={1}>
                    {s.categories.map((cat) => {
                        return (
                            <Chip
                                key={cat.id}
                                label={cat.name}
                                size="small"
                                sx={{
                                    backgroundColor: cat.color,
                                    color: "#ffffff"
                                }}
                            />
                        );
                    })}
                </Box>

            </CardContent>
        </Card>
    );
};
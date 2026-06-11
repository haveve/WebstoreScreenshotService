import { Box, Paper, Slider, TextField, Typography, Stack, Button } from "@mui/material";
import { useMemo, useState } from "react";
import { useDispatch } from "react-redux";
import { addPoints, getPrice } from "../../behavior/basket/reducer";

const MIN_POINTS = 1000;
const MAX_POINTS = 1_000_000;

const clamp = (value: number) =>
    Math.min(MAX_POINTS, Math.max(MIN_POINTS, value));

const formatPoints = (v: number) => v.toLocaleString();

export default function PointsPicker() {
    const [points, setPoints] = useState(25_000);
    const [loading, setLoading] = useState(false);

    const safePoints = useMemo(() => clamp(points), [points]);
    const price = useMemo(() => getPrice(safePoints), [safePoints]);

    const dispatch = useDispatch();

    const handlePurchase = async () => {
        try {
            setLoading(true);
            dispatch(addPoints({ amount: points, price }))
            await new Promise((res) => setTimeout(res, 1200));
        } finally {
            setLoading(false);
        }
    };

    return (
        <Paper sx={{ p: 4, mt: 4 }}>
            <Typography variant="h6" gutterBottom>
                Купівля додаткових балів
            </Typography>

            <Typography variant="body2" color="text.secondary" gutterBottom>
                Оберіть кількість балів для генерації скріншотів
            </Typography>

            <Box sx={{ mt: 3 }}>
                <Slider
                    value={safePoints}
                    min={MIN_POINTS}
                    max={MAX_POINTS}
                    step={1000}
                    onChange={(_, value) => setPoints(value as number)}
                    valueLabelDisplay="auto"
                    valueLabelFormat={(v) => `${formatPoints(v)} pts`}
                />
            </Box>

            <Stack direction={{ xs: "column", md: "row" }} spacing={3} sx={{ mt: 3 }}>
                <TextField
                    label="Бали"
                    type="number"
                    value={safePoints}
                    onChange={(e) => setPoints(Number(e.target.value))}
                    slotProps={{
                        htmlInput: {
                            min: MIN_POINTS,
                            max: MAX_POINTS
                        }
                    }}
                />

                <Box sx={{ flex: 1 }}>
                    <Typography variant="h5">
                        ${price}
                    </Typography>

                    <Typography variant="body2" color="text.secondary">
                        {formatPoints(safePoints)} балів
                    </Typography>

                    <Typography variant="caption" color="text.secondary">
                        ~{Math.floor(safePoints / 7)} скріншотів
                    </Typography>
                </Box>

                <Box sx={{ mt: 4 }}>
                    <Button
                        fullWidth
                        size="large"
                        variant="contained"
                        onClick={handlePurchase}
                        disabled={loading}
                    >
                        {loading
                            ? "Додаємо до кошику..."
                            : `Придбати ${formatPoints(safePoints)} балів за $${price}`}
                    </Button>
                </Box>
            </Stack>
        </Paper>
    );
}
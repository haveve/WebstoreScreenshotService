import {
  Container,
  Card,
  Typography,
  Stack,
  Button,
  Chip,
  Box,
  Dialog,
  DialogTitle,
  DialogActions,
  Divider
} from "@mui/material";
import { useDispatch } from "react-redux";
import { useEffect, useState } from "react";
import { useAppSelector } from "../../behavior/rootReducer";
import {
  loadTransactions,
  refundTransaction
} from "../../behavior/transactions/epic";

export const TransactionsPage = () => {
  const dispatch = useDispatch();
  const tx = useAppSelector(s => s.transactions.items);

  const [refundId, setRefundId] = useState<string | null>(null);

  useEffect(() => {
    dispatch(loadTransactions());
  }, [dispatch]);

  const confirmRefund = (id: string) => {
    dispatch(refundTransaction(id));
    setRefundId(null);
  };

  const getColor = (status: string) => {
    switch (status) {
      case "Succeeded":
        return "success";
      case "Pending":
        return "warning";
      case "Failed":
        return "error";
      case "Refunded":
        return "default";
      default:
        return "default";
    }
  };

  const getStatusLabel = (status: string) => {
    switch (status) {
      case "Succeeded":
        return "Успішно";
      case "Pending":
        return "В обробці";
      case "Failed":
        return "Невдало";
      case "Refunded":
        return "Повернено";
      default:
        return status;
    }
  };

  return (
    <Container maxWidth="md" sx={{ mt: 4 }}>
      <Typography variant="h4" fontWeight={700} gutterBottom>
        Транзакції
      </Typography>

      <Stack spacing={2}>
        {tx.map(item => (
          <Card
            key={item.id}
            sx={{
              p: 2,
              borderRadius: 3,
              boxShadow: "0 4px 16px rgba(0,0,0,0.06)"
            }}
          >
            {/* HEADER ROW */}
            <Box
              sx={{
                display: "flex",
                justifyContent: "space-between",
                alignItems: "center"
              }}
            >
              <Box>
                <Typography variant="h6" fontWeight={700}>
                  ${item.amount}
                </Typography>

                <Typography variant="caption" color="text.secondary">
                  {new Date(item.createdAt).toLocaleString("uk-UA")}
                </Typography>
              </Box>

              <Chip
                label={getStatusLabel(item.status)}
                color={getColor(item.status) as any}
                size="small"
              />
            </Box>

            <Divider sx={{ my: 1.5 }} />

            {/* ACTIONS */}
            <Stack direction="row" justifyContent="space-between" alignItems="center">
              <Typography variant="body2" color="text.secondary">
                ID: {item.id}
              </Typography>

              {item.status === "Succeeded" && (
                <Button
                  size="small"
                  variant="outlined"
                  color="error"
                  onClick={() => setRefundId(item.id)}
                >
                  Повернути кошти
                </Button>
              )}
            </Stack>
          </Card>
        ))}
      </Stack>

      {/* CONFIRMATION MODAL */}
      <Dialog open={!!refundId} onClose={() => setRefundId(null)}>
        <DialogTitle>
          Підтвердити повернення коштів?
        </DialogTitle>

        <DialogActions>
          <Button onClick={() => setRefundId(null)}>
            Скасувати
          </Button>

          <Button
            color="error"
            variant="contained"
            onClick={() => confirmRefund(refundId!)}
          >
            Повернути
          </Button>
        </DialogActions>
      </Dialog>
    </Container>
  );
};
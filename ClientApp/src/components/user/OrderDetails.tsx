import {
  Container,
  Card,
  Typography,
  Stack,
  Chip,
  Divider,
  Box,
  Table,
  TableHead,
  TableRow,
  TableCell,
  TableBody
} from "@mui/material";
import { useEffect } from "react";
import { useParams } from "react-router-dom";
import { useDispatch } from "react-redux";
import { useAppSelector } from "../../behavior/rootReducer";
import { loadOrder } from "../../behavior/order/epic";

export const OrderDetailsPage = () => {
  const { id } = useParams();
  const dispatch = useDispatch();
  const order = useAppSelector(s => s.orders.selected);

  useEffect(() => {
    if (id) dispatch(loadOrder(id));
  }, [id, dispatch]);

  if (!order) return null;

  const getColor = (status: string) => {
    switch (status) {
      case "Paid":
        return "success";
      case "PendingPayment":
        return "warning";
      case "Cancelled":
        return "error";
      default:
        return "default";
    }
  };

  return (
    <Container maxWidth="md" sx={{ mt: 4 }}>
      <Card sx={{ p: 3 }}>

        <Typography variant="h5" fontWeight={700}>
          Замовлення #{order.id}
        </Typography>

        <Chip
          label={
            order.status === "Paid"
              ? "Оплачено"
              : order.status === "PendingPayment"
              ? "Очікує оплату"
              : order.status === "Cancelled"
              ? "Скасовано"
              : order.status
          }
          color={getColor(order.status) as any}
          sx={{ mt: 1 }}
        />

        <Divider sx={{ my: 2 }} />

        <Stack spacing={1}>
          <Typography>
            Сума: <b>${order.totalAmount}</b>
          </Typography>

          <Typography variant="caption">
            Створено: {new Date(order.createdAt).toLocaleString()}
          </Typography>
        </Stack>

        {/* LINES */}
        <Box sx={{ mt: 3 }}>
          <Typography variant="h6">Товари</Typography>

          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Товар</TableCell>
                <TableCell>К-сть</TableCell>
                <TableCell>Ціна</TableCell>
                <TableCell>Всього</TableCell>
              </TableRow>
            </TableHead>

            <TableBody>
              {order.lines.map(l => (
                <TableRow key={l.id}>
                  <TableCell>{l.productName}</TableCell>
                  <TableCell>{l.quantity}</TableCell>
                  <TableCell>${l.unitPrice}</TableCell>
                  <TableCell>
                    ${l.quantity * l.unitPrice}
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </Box>
      </Card>
    </Container>
  );
};
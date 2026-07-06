import {
  Container,
  Card,
  Typography,
  Stack,
  TextField,
  MenuItem,
  Select,
  Chip,
  Table,
  TableHead,
  TableRow,
  TableCell,
  TableBody
} from "@mui/material";
import { useEffect, useMemo, useState } from "react";
import { useDispatch } from "react-redux";
import { useAppSelector } from "../../behavior/rootReducer";
import { loadOrders } from "../../behavior/order/epic";
import { Link } from "react-router-dom";

export const OrdersPage = () => {
  const dispatch = useDispatch();
  const orders = useAppSelector(s => s.orders.items);

  const [search, setSearch] = useState("");
  const [status, setStatus] = useState("all");
  const [sort, setSort] = useState("createdAt");

  useEffect(() => {
    dispatch(loadOrders());
  }, [dispatch]);

  const filtered = useMemo(() => {
    return orders
      .filter(o =>
        o.id.toLowerCase().includes(search.toLowerCase())
      )
      .filter(o => status === "all" ? true : o.status === status)
      .sort((a, b) => {
        if (sort === "createdAt") {
          return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
        }
        if (sort === "status") {
          return a.status.localeCompare(b.status);
        }
        return 0;
      });
  }, [orders, search, status, sort]);

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
      <Typography variant="h4" gutterBottom>
        Замовлення
      </Typography>

      {/* FILTERS */}
      <Stack direction="row" spacing={2} sx={{ mb: 2 }}>
        <TextField
          size="small"
          label="Пошук за ID замовлення"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />

        <Select
          size="small"
          value={status}
          onChange={(e) => setStatus(e.target.value)}
        >
          <MenuItem value="all">Всі</MenuItem>
          <MenuItem value="Paid">Оплачено</MenuItem>
          <MenuItem value="PendingPayment">Очікує оплату</MenuItem>
          <MenuItem value="Cancelled">Скасовано</MenuItem>
        </Select>

        <Select
          size="small"
          value={sort}
          onChange={(e) => setSort(e.target.value)}
        >
          <MenuItem value="createdAt">Спочатку нові</MenuItem>
          <MenuItem value="status">За статусом</MenuItem>
        </Select>
      </Stack>

      {/* TABLE */}
      <Card>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Замовлення</TableCell>
              <TableCell>Статус</TableCell>
              <TableCell>Сума</TableCell>
              <TableCell>Дата</TableCell>
            </TableRow>
          </TableHead>

          <TableBody>
            {filtered.map(o => (
              <TableRow key={o.id} hover>
                <TableCell>
                  <Link to={`/my-account/orders/${o.id}`}>
                    {o.id}
                  </Link>
                </TableCell>

                <TableCell>
                  <Chip
                    label={
                      o.status === "Paid"
                        ? "Оплачено"
                        : o.status === "PendingPayment"
                        ? "Очікує оплату"
                        : o.status === "Cancelled"
                        ? "Скасовано"
                        : o.status
                    }
                    color={getColor(o.status) as any}
                  />
                </TableCell>

                <TableCell>
                  ${o.totalAmount}
                </TableCell>

                <TableCell>
                  {new Date(o.createdAt).toLocaleString()}
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </Card>
    </Container>
  );
};
import { Route, BrowserRouter as Router, Routes } from "react-router-dom";
import { Provider } from 'react-redux'
import store from './behavior/rootReducer'
import { AppRoutes } from "./components";
import AdminAppRoutes from "./components/admin/AppRoutes";
import { ThemeContextProvider } from "./components/ThemeSettings";
import { Box } from "@mui/material";
import './localization';

const App = () => {
  return (
    <Provider store={store}>
      <Router>
        <ThemeContextProvider>
          <Box
            sx={{
              display: "flex",
              flexDirection: "column",
              minHeight: "100vh", // займає всю висоту вікна
            }}
          >

            {/* Основний контент */}
            <Routes>
              {/*<Route path="/*" element={<AppRoutes />} />*/}
              <Route path="/*" element={<AppRoutes />} />
            </Routes>

            {/* Футер внизу */}
          </Box>
        </ThemeContextProvider>
      </Router>
    </Provider>
  );
};

export default App;

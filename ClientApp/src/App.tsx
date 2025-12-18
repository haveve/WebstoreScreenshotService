import { BrowserRouter as Router } from "react-router-dom";
import { Provider } from 'react-redux'
import store from './behavior/rootReducer'
import Navigation from "./components/Navigation";
import { AppRoutes } from "./components";
import './localization.ts';
import { ThemeContextProvider } from "./components/ThemeSettings";
import Footer from "./components/Footer";
import { Box } from "@mui/material";

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
            {/* Навігація зверху */}
            <Navigation />

            {/* Основний контент */}
            <Box component="main" sx={{ flex: 1 }}>
              <AppRoutes />
            </Box>

            {/* Футер внизу */}
            <Footer />
          </Box>
        </ThemeContextProvider>
      </Router>
    </Provider>
  );
};

export default App;

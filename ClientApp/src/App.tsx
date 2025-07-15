import { BrowserRouter as Router } from "react-router-dom";
import 'bootstrap/dist/css/bootstrap.min.css';
import { Provider } from 'react-redux'
import store from './behavior/rootReducer'
import Navigation from "./components/Navigation";
import { AppRoutes } from "./components";
import './localization.ts';

const App = () => {
  return (
    <Provider store={store}>
      <Router>
        <Navigation />
        <AppRoutes />
      </Router>
    </Provider>
  );
};

export default App;

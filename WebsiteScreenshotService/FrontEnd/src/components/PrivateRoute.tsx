import { Navigate } from "react-router-dom";

type PrivateRouteProps = {
    validate: () => boolean;
    toPath: string;
    element: React.ReactElement;
}

const PrivateRoute = ({ element, toPath, validate }: PrivateRouteProps) => {
    return validate() ? (
        element
    ) : (
        <Navigate to={toPath} replace />
    )
};

export default PrivateRoute;
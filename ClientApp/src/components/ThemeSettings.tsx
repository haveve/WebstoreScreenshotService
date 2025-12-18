import {
  useState,
  createContext,
  useContext,
  useMemo,
  ReactNode,
  ReactElement,
} from "react";
import {
  ThemeProvider,
  createTheme,
  CssBaseline,
  Theme,
} from "@mui/material";
import { getCookie, setCookie } from './helpers';

/* ------------------ THEME CONTEXT ------------------ */

const THEME_COOKIE = "theme_mode";
const COOKIE_EXP_DAYS = 365;

type ThemeMode = "light" | "dark";

interface ThemeContextProps {
  mode: ThemeMode;
  toggleTheme: () => void;
  theme: Theme;
}

const ThemeContext = createContext<ThemeContextProps | undefined>(undefined);

export const useThemeMode = (): ThemeContextProps => {
  const context = useContext(ThemeContext);
  if (!context) {
    throw new Error("useThemeMode must be used within a ThemeContextProvider");
  }
  return context;
};

/* ------------------ THEME CREATION ------------------ */
type ThemeColors = {
  primaryMain: string;
  primaryContrast: string;
  secondaryMain: string;
  secondaryContrast: string;
  backgroundDefault: string;
  backgroundPaper: string;
  textPrimary: string;
  textSecondary: string;
  shadowColor: string;
  borderDefault: string;   // for inputs/borders
  borderFocus: string;
  borderError: string;
};

const getThemeColors = (mode: ThemeMode): ThemeColors => {
  switch (mode) {
    case "light":
      return {
        primaryMain: "#0077b6",        // deep ocean blue
        primaryContrast: "#ffffff",
        secondaryMain: "#00b4d8",      // aqua accent
        secondaryContrast: "#ffffff",
        backgroundDefault: "#f9fbfd",  // subtle off-white
        backgroundPaper: "#ffffff",
        textPrimary: "#002244",        // strong navy for readability
        textSecondary: "#334155",      // softer gray-blue for secondary
        shadowColor: "0, 0, 0",
        borderDefault: "#94a3b8",      // neutral border
        borderFocus: "#00b4d8",        // matches secondary
        borderError: "#d32f2f",        // standard error red
      };

    case "dark":
      return {
        primaryMain: "#00b4d8",        // bright aqua for highlights
        primaryContrast: "#00111a",    // very dark contrast
        secondaryMain: "#90e0ef",      // soft cyan
        secondaryContrast: "#00111a",
        backgroundDefault: "#0a192f",  // deep navy
        backgroundPaper: "#112240",    // slightly lighter navy
        textPrimary: "#e6f1ff",        // bright but not pure white
        textSecondary: "#a8c3d9",      // muted sky blue-gray
        shadowColor: "0, 0, 0",
        borderDefault: "#64748b",      // dark neutral border
        borderFocus: "#00b4d8",        // consistent with primary
        borderError: "#f87171",        // softer red for dark bg
      };

    default:
      throw new Error(`Unsupported theme mode: ${mode}`);
  }
};

export const createCustomTheme = (colors: ThemeColors, mode: ThemeMode): Theme =>
  createTheme({
    palette: {
      mode,
      primary: {
        main: colors.primaryMain,
        contrastText: colors.primaryContrast,
      },
      secondary: {
        main: colors.secondaryMain,
        contrastText: colors.secondaryContrast,
      },
      background: {
        default: colors.backgroundDefault,
        paper: colors.backgroundPaper,
      },
      text: {
        primary: colors.textPrimary,
        secondary: colors.textSecondary,
      },
      error: {
        main: colors.borderError,
      },
    },
    shape: { borderRadius: 8 },
    components: {
      MuiButton: {
        styleOverrides: {
          root: {
            textTransform: "none",
            borderRadius: 8,
            boxShadow: "none",
            "&:hover": {
              boxShadow: `0 2px 4px rgba(${colors.shadowColor}, 0.2)`,
            },
          },
        },
      },
      MuiCard: {
        styleOverrides: {
          root: {
            borderRadius: 12,
            boxShadow: `0 2px 8px rgba(${colors.shadowColor}, 0.2)`,
            transition: "box-shadow 0.3s ease",
          },
        },
      },
      MuiAppBar: {
        styleOverrides: {
          root: {
            boxShadow: `0 2px 4px rgba(${colors.shadowColor}, 0.1)`,
            transition: "background-color 0.3s ease",
          },
        },
      },

      // Form elements
      MuiOutlinedInput: {
        styleOverrides: {
          root: {
            "& fieldset": {
              borderColor: colors.borderDefault,
            },
            "&:hover fieldset": {
              borderColor: colors.borderFocus,
            },
            "&.Mui-focused fieldset": {
              borderColor: colors.borderFocus,
            },
            "&.Mui-error fieldset": {
              borderColor: colors.borderError,
            },
          },
        },
      },
      MuiInputLabel: {
        styleOverrides: {
          root: {
            color: colors.textSecondary,
            "&.Mui-focused": {
              color: colors.borderFocus,
            },
            "&.Mui-error": {
              color: colors.borderError,
            },
          },
        },
      },
      MuiSelect: {
        styleOverrides: {
          outlined: {
            "& fieldset": {
              borderColor: colors.borderDefault,
            },
            "&:hover fieldset": {
              borderColor: colors.borderFocus,
            },
            "&.Mui-focused fieldset": {
              borderColor: colors.borderFocus,
            },
            "&.Mui-error fieldset": {
              borderColor: colors.borderError,
            },
          },
        },
      },
      MuiSwitch: {
        styleOverrides: {
          switchBase: {
            color: colors.secondaryMain,
          },
          track: {
            backgroundColor: colors.borderDefault,
          },
        },
      },
      MuiAutocomplete: {
        styleOverrides: {
          root: {
            "& .MuiOutlinedInput-root fieldset": {
              borderColor: colors.borderDefault,
            },
            "&:hover .MuiOutlinedInput-root fieldset": {
              borderColor: colors.borderFocus,
            },
            "&.Mui-focused .MuiOutlinedInput-root fieldset": {
              borderColor: colors.borderFocus,
            },
            "&.Mui-error .MuiOutlinedInput-root fieldset": {
              borderColor: colors.borderError,
            },
          },
        },
      },
    },
  });

/* ------------------ PROVIDER ------------------ */

type ThemeContextProviderProps = {
  children: ReactNode;
}

export const ThemeContextProvider = ({
  children,
}: ThemeContextProviderProps): ReactElement => {
  const [mode, setMode] = useState<ThemeMode>(() => {
    const saved = getCookie(THEME_COOKIE);
    return saved === "dark" ? "dark" : "light";
  });

  const toggleTheme = () => {
    setMode((prevMode) => {
      const newMode = prevMode === "light" ? "dark" : "light";
      setCookie(THEME_COOKIE, newMode, COOKIE_EXP_DAYS);
      return newMode;
    });
  };


  const theme = useMemo(() => createCustomTheme(getThemeColors(mode), mode), [mode]);

  const value: ThemeContextProps = { mode, toggleTheme, theme };

  return (
    <ThemeContext.Provider value={value}>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        {children}
      </ThemeProvider>
    </ThemeContext.Provider>
  );
};
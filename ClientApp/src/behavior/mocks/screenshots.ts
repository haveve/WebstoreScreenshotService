import { Screenshot, ScreenshotState, ScreenshotType } from "../types";

export const mockCategories = Array.from({ length: 35 }).map((_, i) => {
    const baseNames = [
        "Work", "Personal", "Ideas", "Bug", "Design", "Marketing", "Sales",
        "Finance", "Research", "AI", "Frontend", "Backend", "Mobile",
        "DevOps", "Security", "UX", "UI", "Product", "Growth", "Analytics"
    ];

    const name = baseNames[i % baseNames.length] + (i >= baseNames.length ? ` ${Math.floor(i / baseNames.length) + 1}` : "");

    const colors = [
        "#1976d2", "#2e7d32", "#ed6c02", "#9c27b0", "#d32f2f",
        "#0288d1", "#00796b", "#5d4037", "#455a64", "#f57c00",
        "#388e3c", "#c2185b", "#7b1fa2", "#512da8", "#303f9f",
        "#0097a7", "#00acc1", "#43a047", "#8bc34a", "#c0ca33",
        "#fdd835", "#ffb300", "#fb8c00", "#ef5350", "#e53935",
        "#d81b60", "#ab47bc", "#6a1b9a", "#8e24aa", "#3949ab",
        "#1e88e5", "#039be5", "#00bcd4", "#26a69a", "#66bb6a",
        "#9ccc65", "#d4e157", "#ffee58", "#ffca28", "#ffa726"
    ];

    return {
        id: `${i + 1}`,
        name,
        color: colors[i % colors.length]
    };
});

// helper arrays for realism
const titles = [
    "Login Bug", "UI Issue", "Landing Page", "Dashboard Error",
    "API Failure", "Button Misaligned", "Performance Drop",
    "Data Mismatch", "Dark Mode Bug", "Mobile Layout Issue"
];

const descriptions = [
    "Something is broken here",
    "UI looks weird on smaller screens",
    "Needs redesign and cleanup",
    "Unexpected behavior when interacting",
    "API response is inconsistent",
    "Layout shifts unexpectedly",
    "Performance is slower than expected",
    "Data is not syncing properly"
];

function getRandomCategories() {
    const count = Math.floor(Math.random() * 3) + 1; // 1–3 categories
    const shuffled = [...mockCategories].sort(() => 0.5 - Math.random());
    return shuffled.slice(0, count);
}

const imagePool = [
    "https://content2.rozetka.com.ua/goods/images/big/594364394.jpg",
    "https://content2.rozetka.com.ua/goods/images/big/594348666.jpg",
    "https://content2.rozetka.com.ua/goods/images/big/594348310.jpg",
    "https://content.rozetka.com.ua/goods/images/big/594345324.jpg",
    "https://content2.rozetka.com.ua/goods/images/big/594345429.jpg",
    "https://ztu.edu.ua/img/mainpage/header/photo8.jpg"
];

export const mockScreenshots: Screenshot[] =
    Array.from({ length: 100 }).map((_, i) => ({
        id: `${i + 1}`,
        url: imagePool[i % imagePool.length],
        websiteUrl: `https://rozetka.com.ua/`,
        createdAt: new Date(
            Date.now() - i * 1000 * 60 * 30 // every 30 min
        ).toISOString(),
        state: ScreenshotState.Successful,
        type: ScreenshotType.Png,
        title: titles[i % titles.length],
        description: descriptions[i % descriptions.length],
        categories: getRandomCategories()
    }));
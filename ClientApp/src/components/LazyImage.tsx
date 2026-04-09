import { useEffect, useRef, useState } from "react";
import { Skeleton } from "@mui/material";

type Props = {
    src: string;
    alt?: string;
    height?: number;
};

const LazyImage = ({ src, alt, height = 300 }: Props) => {
    const ref = useRef<HTMLDivElement | null>(null);
    const [visible, setVisible] = useState(false);
    const [loaded, setLoaded] = useState(false);

    useEffect(() => {
        const node = ref.current;
        if (!node) return;

        const observer = new IntersectionObserver(
            ([entry]) => {
                if (entry.isIntersecting) {
                    setVisible(true);
                    observer.disconnect();
                }
            },
            { threshold: 0.1 }
        );

        observer.observe(node);

        return () => observer.disconnect();
    }, []);

    return (
        <div ref={ref}>
            {!visible || !loaded ? (
                <Skeleton
                    variant="rectangular"
                    height={height}
                    animation="wave"
                />
            ) : null}

            {visible && (
                <img
                    src={src}
                    alt={alt}
                    style={{
                        width: "100%",
                        height,
                        objectFit: "contain",
                        display: loaded ? "block" : "none"
                    }}
                    onLoad={() => setLoaded(true)}
                />
            )}
        </div>
    );
};

export default LazyImage;
import type { Track } from "../dataLoader";

export function renderMaster(container: HTMLElement, edition: string, tracks: Track[]): void {
    // Edition label
    const editionLabel = container.querySelector(".edition-label") as HTMLElement;
    if (editionLabel) {
        editionLabel.textContent = `Edition ${edition}`;
    }

    // Track list UL
    const ul = container.querySelector(".track-list") as HTMLElement;
    if (!ul) {
        console.error("track-list element not found in master HTML");
        return;
    }

    // Clear old items
    ul.innerHTML = "";

    // Render each track
    tracks.forEach(track => {
        const li = document.createElement("li");
        li.className = "track-item";

        const link = document.createElement("a");
        link.className = "track-link";
        link.href = `#/${edition}/${track.slug}`;

        // Position
        const pos = document.createElement("span");
        pos.className = "track-position";
        pos.textContent = `${track.position}.`;

        // Title + artist
        const main = document.createElement("div");
        main.className = "track-main";

        const title = document.createElement("span");
        title.className = "track-title";
        title.textContent = track.title;

        const artist = document.createElement("span");
        artist.className = "track-artist";
        artist.textContent = track.artist;

        main.appendChild(title);
        main.appendChild(artist);

        // Delta icon + value
        const icon = document.createElement("span");
        icon.className = "material-symbols-outlined track-delta-icon";

        const delta = document.createElement("span");
        delta.className = "track-delta-value";

        if (!track.delta || track.delta === 0) {
            icon.textContent = "drag_handle";
            delta.textContent = "";
        } else if (track.delta > 0) {
            icon.textContent = "arrow_upward";
            delta.textContent = `${track.delta}`;
        } else {
            icon.textContent = "arrow_downward";
            delta.textContent = `${Math.abs(track.delta)}`;
        }

        icon.style.color = track.colour;

        // Build structure
        link.appendChild(pos);
        link.appendChild(main);
        link.appendChild(icon);
        link.appendChild(delta);

        li.appendChild(link);
        ul.appendChild(li);
    });
}

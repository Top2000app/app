import {type Track} from "../dataLoader";
import {TrackListingDeltaType} from "../trackListingDeltaType.ts";

const deltaIcons: Record<TrackListingDeltaType, string> = {
    [TrackListingDeltaType.NoChange]: "equal",
    [TrackListingDeltaType.Increased]: "arrow_upward",
    [TrackListingDeltaType.Decreased]: "arrow_downward",
    [TrackListingDeltaType.New]: "flag",
    [TrackListingDeltaType.Recurring]: "replay"
};

const deltaIconsColour: Record<TrackListingDeltaType, string> = {
    [TrackListingDeltaType.NoChange]: "stat-equal",
    [TrackListingDeltaType.Increased]: "stat-up",
    [TrackListingDeltaType.Decreased]: "stat-down",
    [TrackListingDeltaType.New]: "stat-flag",
    [TrackListingDeltaType.Recurring]: "stat-repeat"
};

export function renderMaster(
    container: HTMLElement,
    edition: string,
    tracks: Track[]
): void {

    const editionLabel = container.querySelector(".edition-label") as HTMLElement;
    if (editionLabel) {
        editionLabel.textContent = `Edition ${edition}`;
    }

    const ul = container.querySelector(".track-list") as HTMLElement;
    if (!ul) {
        console.error("track-list element not found in master HTML");
        return;
    }

    const template = document.getElementById(
        "track-item-template"
    ) as HTMLTemplateElement;

    if (!template) {
        console.error("track-item-template not found");
        return;
    }

    ul.innerHTML = "";

    tracks.forEach(track => {

        // Clone template
        const fragment = template.content.cloneNode(true) as DocumentFragment;

        // Get elements from clone
        const link = fragment.querySelector(".track-link") as HTMLAnchorElement;

        const positionValue = fragment.querySelector(
            ".track-position-value"
        ) as HTMLElement;

        const title = fragment.querySelector(
            ".track-title"
        ) as HTMLElement;

        const artist = fragment.querySelector(
            ".track-artist"
        ) as HTMLElement;

        const deltaIcon = fragment.querySelector(
            ".track-delta-icon"
        ) as HTMLElement;

        const deltaValue = fragment.querySelector(
            ".track-delta-value"
        ) as HTMLElement;

        // Populate data
        link.href = `#/${edition}/${track.slug}`;

        positionValue.textContent = track.position.toString();

        title.textContent = track.title;
        artist.textContent = track.artist;
        deltaIcon.textContent = deltaIcons[track.icon];
        deltaIcon.classList.add(deltaIconsColour[track.icon]);
        deltaValue.classList.add(deltaIconsColour[track.icon]);
        deltaValue.textContent = track.delta?.toString() ?? "";
     
        ul.appendChild(fragment);
    });


}
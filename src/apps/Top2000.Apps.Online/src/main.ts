import "./styles/main.scss";
import { onRouteChange, parseRoute } from "./router";
import { renderMaster } from "./views/master";
import { renderDetail } from "./views/detail";
import { loadEdition, type Track } from "./dataLoader";

const master = document.querySelector(".master") as HTMLElement;
const detail = document.querySelector(".detail") as HTMLElement;

let cachedTracks: Track[] = [];
const themeToggle = document.getElementById("theme-toggle") as HTMLElement;

themeToggle.addEventListener("click", () => {
    document.body.classList.toggle("light-theme");
});

const groupMenu = document.getElementById("group-list-menu") as HTMLElement;

groupMenu.addEventListener("click", (e) => {
    const target = e.target as HTMLElement;

    // Was a dropdown item clicked?
    const item = target.closest(".dropdown-item") as HTMLElement | null;
    console.log(item);
    if (!item) return;

    e.preventDefault();

    const group = item.dataset.group;
    console.log(group);
    if (!group) return;

    document.getElementById(`group-${group}`)?.scrollIntoView({
        behavior: "auto",
        block: "start"
    });
});

async function updateUI(): Promise<void> {
    const { edition, slug } = parseRoute();

    // Load edition data
    if (!cachedTracks.length) {
        try {
            cachedTracks = await loadEdition(edition);
        } catch (err) {
            master.innerHTML = `<p>Edition ${edition} not found.</p>`;
            return;
        }
    }

    const isSmall = window.matchMedia("(max-width: 800px)").matches;

    if (!slug) {
        renderMaster(master, String(edition), cachedTracks);
        master.classList.remove("hidden");
        detail.classList.add("hidden");
    } else {
        const track = cachedTracks.find(t => t.slug === slug);
        renderDetail(detail, String(edition), slug, track);

        if (isSmall) {
            master.classList.add("hidden");
            detail.classList.remove("hidden");
        } else {
            master.classList.remove("hidden");
            detail.classList.remove("hidden");
        }
    }
}

onRouteChange(updateUI);
updateUI();

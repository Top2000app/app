export const TrackListingDeltaType = {
    NoChange: 0,
    Increased: 1,
    Decreased: 2,
    New: 3,
    Recurring: 4
} as const;

export type TrackListingDeltaType =
    (typeof TrackListingDeltaType)[keyof typeof TrackListingDeltaType];
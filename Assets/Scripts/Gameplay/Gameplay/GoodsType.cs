using UnityEngine;


//all neighbouring hexes that can produce the same goods are one field (up to certain size)
public enum GoodsType
{
    Flax,
    //Flax needs fertile, well-drained soil, best in moderate, temperate climates with sufficient rainfall. 
    //It struggled in: Very dry zones (too little water), Cold northern areas (short growing seasons), Poor, rocky soils (low fertility).
    //Can be turned into oil and textiles.
    Clay,
    //very abundant in river valleys and deltas.
    //Can be turned into pottery and bricks.
    Timber,
    //Too heavy for long-distance transport.
    //Can be turned into buildings, fuel (cooking, heating, pottery), boats.
    Obsidian,
    //Can be turned into tools for art.
    Flint,
    //Can be turned into tools and weapons.
    Shells,
    //Can be turned into jewelry.
    Ochre,
    // Common but some high-quality sources were traded.
    // Used as pigment for art, body paint, and rituals.
    Malachite_azurite,
    // Copper minerals, rarer and traded.
    // Used as pigments; later as copper ores.
    Manganese_oxides,
    // Less common mineral pigments, sometimes traded.
    // Used for black/purple pigments in art and ritual.
    Hematite_limonite,
    // Iron oxides; traded when high-quality.
    // Used as red/yellow pigments, symbolic items.
    Salt,
    //Can be turned into seasoning and preservation.
}   
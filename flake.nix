{
  description = "Note taking system";

  inputs = {
    # Pointing to the current stable release of nixpkgs. You can
    # customize this to point to an older version or unstable if you
    # like everything shining.
    #
    # E.g.
    #
    # nixpkgs.url = "github:NixOS/nixpkgs/unstable";
    nixpkgs.url = "github:NixOS/nixpkgs/24.05";

    utils.url = "github:numtide/flake-utils";
  };

  outputs = { self, nixpkgs, ... }@inputs: inputs.utils.lib.eachSystem [
    # Add the system/architecture you would like to support here. Note that not
    # all packages in the official nixpkgs support all platforms.
    "x86_64-linux" "i686-linux" "aarch64-linux" "x86_64-darwin"
  ] (system: let
    pkgs = import nixpkgs {
      inherit system;

      # Add overlays here if you need to override the nixpkgs
      # official packages.
      overlays = [];
      
      # Uncomment this if you need unfree software (e.g. cuda) for
      # your project.
      #
      # config.allowUnfree = true;
    };
    fhs = pkgs.buildFHSUserEnv {
      name = "fhs-shell";
      targetPkgs = pkgs: [
        pkgs.dotnet-sdk_8
        pkgs.wayland
        pkgs.wayland-scanner
        pkgs.wayland-protocols
        pkgs.libxkbcommon
        pkgs.xorg.libX11
        pkgs.xorg.libXrandr
        pkgs.xorg.libXinerama
        pkgs.xorg.libXcursor
        pkgs.xorg.libXi
        pkgs.xorg.libXext
        pkgs.libffi
	pkgs.raylib
      ];
    };
  in {
    devShells.default = fhs.env;
    #packages.default = pkgs.callPackage ./default.nix {};
  });
}

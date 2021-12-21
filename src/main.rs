use hudsucker::{ certificate_authority::RcgenAuthority, * };
use argh::FromArgs;

#[derive(FromArgs)]
#[argh(description = "omp arguments")]
struct OmpArgs {
    #[argh(
        option,
        description = "the proxy mode to use. can be \"mantle\", \"optimantle\", or \"mantlefine\". leave blank to default to \"mantle\".",
        short = 'm'
    )]
    mode: Option<String>,

    #[argh(
        switch,
        description = "provides help with proxy modes (see \"--mode\"). prints a simple message that describes proxy mode functionality."
    )]
    mode_help: bool
}

#[tokio::main]
async fn main() {
    println!("Welcome to OpenMantleProxy!");
    println!("This is a Free/Libre proxy to recreate Mantle's functionality, fully transparent.");

    let args:OmpArgs = argh::from_env();

    println!("Read launch arguments.");

    if args.mode_help {
        println!(
"Proxy modes:
 - mantle: Default proxy configuration, displays only only Mantle capes.
 - optimantle: Displays Mantle capes if an OptiFine cape could not be resolved.
 - mantlefine: Displays OptiFine capes if a Mantle cape could not be resolved."
        );

        return;
    }

    let mode:String;

    if args.mode.is_none() {
        println!("Could not resolve argument \"--mode\" (\"-m\")");
        println!("Setting mode to default (\"mantle\")");
        mode = "mantle".to_string();
    }
    else {
        mode = args.mode.unwrap();
    }

    println!("Using proxy mode: {}", mode);

    println!("Parsing certificate.");

    let mut cert_bytes:&[u8] = include_bytes!("../certs/cert.pem");

    let cert = rustls::Certificate(
        rustls_pemfile::certs(&mut cert_bytes)
            .expect("Failed to parse certificate, aborting")
            .remove(0)
    );

    println!("Parsing key.");

    let mut key_bytes:&[u8] = include_bytes!("../certs/key.pem");

    let key = rustls::PrivateKey(
        rustls_pemfile::pkcs8_private_keys(&mut key_bytes)
            .expect("Failed to parse key, aborting")
            .remove(0)
    );
    println!("Creating certificate authority.");
    let ca:RcgenAuthority = RcgenAuthority::new(
        key, cert, 1_000
    ).expect("Failed to create certificate authority");

    println!("Building proxy.");
    let proxy = ProxyBuilder::new()
        .with_addr(std::net::SocketAddr::from(([127, 0, 0, 1], 3000)))
        .with_rustls_client()
        .with_ca(ca)
        .build();

    println!("Starting proxy.");

    if let Err(e) = proxy.start(shutdown_signal()).await {
        println!("{}", e);
    }
}

async fn shutdown_signal() {
    tokio::signal::ctrl_c()
    .await
    .expect("Failed to install CTRL+C signal handler");
}

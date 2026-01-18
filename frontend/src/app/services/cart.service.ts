import { Injectable, PLATFORM_ID, Inject } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { isPlatformBrowser } from '@angular/common';
import { Uniforme } from '../models/uniforme.model';

@Injectable({
    providedIn: 'root'
})
export class CartService {
    private itemsSubject = new BehaviorSubject<Uniforme[]>([]);
    public items$ = this.itemsSubject.asObservable();

    private readonly STORAGE_KEY = 'cart_items';

    constructor(@Inject(PLATFORM_ID) private platformId: Object) {
        this.loadCart();
    }

    private loadCart() {
        if (isPlatformBrowser(this.platformId)) {
            const saved = localStorage.getItem(this.STORAGE_KEY);
            if (saved) {
                try {
                    this.itemsSubject.next(JSON.parse(saved));
                } catch (e) {
                    console.error('Error loading cart', e);
                }
            }
        }
    }

    private saveCart(items: Uniforme[]) {
        if (isPlatformBrowser(this.platformId)) {
            localStorage.setItem(this.STORAGE_KEY, JSON.stringify(items));
            this.itemsSubject.next(items);
        }
    }

    addToCart(uniforme: Uniforme) {
        const currentItems = this.itemsSubject.value;
        // Evitar duplicados (aunque los uniformes son únicos, validamos por si acaso)
        if (!currentItems.find(i => i.idUniforme === uniforme.idUniforme)) {
            const newItems = [...currentItems, uniforme];
            this.saveCart(newItems);
        }
    }

    removeFromCart(idUniforme: number) {
        const currentItems = this.itemsSubject.value;
        const newItems = currentItems.filter(i => i.idUniforme !== idUniforme);
        this.saveCart(newItems);
    }

    clearCart() {
        this.saveCart([]);
    }

    getItems(): Uniforme[] {
        return this.itemsSubject.value;
    }

    getTotal(): number {
        return this.itemsSubject.value.reduce((acc, item) => acc + item.precio, 0);
    }

    isInCart(idUniforme: number): boolean {
        return this.itemsSubject.value.some(i => i.idUniforme === idUniforme);
    }
}
